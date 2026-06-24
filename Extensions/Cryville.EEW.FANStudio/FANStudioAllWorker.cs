using Cryville.EEW.ComponentModel;
using Cryville.EEW.FANStudio.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.FANStudio {
	public class FANStudioAllWorker : WebSocketWorker, ISourceWorker, IPropertiesHolder {
		internal static readonly Dictionary<Type, string> _sourceNameMap = [];
		static readonly Dictionary<string, JsonTypeInfo> _sourceTypeInfoMap = [];
		static readonly Dictionary<FANStudioSource, string> _sourceEnumNameMap = [];
		static FANStudioAllWorker() {
			foreach (var p in typeof(FANStudioInitialAllMessage).GetProperties()) {
				var propertyType = p.PropertyType;
				if (!propertyType.IsGenericType || propertyType.GetGenericTypeDefinition() != typeof(FANStudioData<>)) continue;
				var type = propertyType.GetGenericArguments()[0];
				var name = p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name;
				var typeInfo = SerializerContext.Default.GetTypeInfo(type);
				if (typeInfo == null) continue;
				_sourceNameMap.Add(type, name);
				_sourceTypeInfoMap.Add(name, typeInfo);
				if (Enum.TryParse<FANStudioSource>(type.Name, out var sourceEnum)) {
					_sourceEnumNameMap.Add(sourceEnum, name);
				}
			}
		}

		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<object>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		[Obsolete("Filter is always whitelist.")]
		[Browsable(false)]
		public bool IsFilterWhitelist { get; set; }

		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<FANStudioSource> Filter {
			get => m_filter;
			set {
				m_filter = value;
				MapFilter();
			}
		}
		[MemberNotNull(nameof(_sourceFilter))]
		void MapFilter() {
			_sourceFilter = [.. m_filter.Select(i => _sourceEnumNameMap.TryGetValue(i, out string? source) ? source : "")];
		}
		ISet<FANStudioSource> m_filter = (HashSet<FANStudioSource>)[];
		HashSet<string> _sourceFilter;
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IEnumerable<string> filter) => _sourceFilter = [.. filter];

		public FANStudioAllWorker(Uri uri) : base(uri) {
			MapFilter();
		}

		readonly Dictionary<string, (HashSet<string>, Queue<string>)> _history = [];

		protected override async Task Handle(Stream stream, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FANStudioMessage, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				if (e is FANStudioInitialAllMessage initialAllMsg) {
					foreach (var ev in initialAllMsg.Enumerate()) {
						if (ev.Data is not object data)
							continue;
						if (!_sourceNameMap.TryGetValue(data.GetType(), out var source))
							continue;
						if (!_sourceFilter.Contains(source))
							continue;
						HandleData(source, data, ev.MD5, false);
					}
				}
				else if (e is FANStudioUpdateMessage updateMsg) {
					string source = updateMsg.Source;
					if (!_sourceFilter.Contains(source))
						return;
					if (!_sourceTypeInfoMap.TryGetValue(source, out var typeInfo))
						return;
					var ev = updateMsg.Data.Deserialize(typeInfo) ?? throw new JsonException("Null event.");
					HandleData(source, ev, updateMsg.MD5, true);
				}
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
			catch (NotSupportedException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		void HandleData(string source, object ev, string? hash, bool isUpdate) {
			hash ??= ev.GetHashCode().ToString("x8", CultureInfo.InvariantCulture);
			if (!_history.TryGetValue(source, out var history)) {
				_history.Add(source, history = ([], []));
			}
			var (historySet, historyList) = history;
			if (!historySet.Add(hash)) return;
			historyList.Enqueue(hash);
			if (isUpdate || historyList.Count > 1) {
				if (historyList.Count > 10) {
					historySet.Remove(historyList.Dequeue());
				}
				Received?.Invoke(this, ev);
			}
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
