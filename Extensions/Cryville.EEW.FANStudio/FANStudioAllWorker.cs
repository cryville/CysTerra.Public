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
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.FANStudio {
	public class FANStudioAllWorker : WebSocketWorker, ISourceWorker, IPropertiesHolder {
		public virtual string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

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
		protected virtual IEnumerable<FANStudioSource> EnumerateFilter() => m_filter;
		protected void MapFilter() {
			_sourceFilter = [.. EnumerateFilter().Select(i => _typeInfoProvider.TryGetSource(i, out string? source) ? source : "")];
		}
		ISet<FANStudioSource> m_filter = (HashSet<FANStudioSource>)[];
		HashSet<string> _sourceFilter = [];
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IEnumerable<string> filter) => _sourceFilter = [.. filter];

		readonly FANStudioSourceTypeInfoProvider _typeInfoProvider;

		public FANStudioAllWorker(Uri uri, FANStudioSourceTypeInfoProvider typeInfoProvider) : base(uri) {
			_typeInfoProvider = typeInfoProvider;
		}

		readonly Dictionary<string, (HashSet<string>, Queue<string>)> _history = [];

		protected override async Task Handle(Stream stream, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FANStudioMessage, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				HandleMessage(e);
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
			catch (NotSupportedException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		protected virtual void HandleMessage(FANStudioMessage e) {
			if (e is FANStudioInitialAllMessage initialAllMsg) {
				if (initialAllMsg.Data is not { } allData)
					return;
				foreach (var data in allData) {
					string source = data.Key;
					if (!_typeInfoProvider.TryGetWrappedDataTypeInfo(source, out var typeInfo))
						continue;
					if (!_sourceFilter.Contains(source))
						continue;
					var ev = (IFANStudioData<object>)(data.Value.Deserialize(typeInfo) ?? throw new JsonException("Null event."));
					HandleData(source, ev.Data, ev.MD5, false);
				}
			}
			else if (e is FANStudioUpdateMessage updateMsg) {
				string source = updateMsg.Source;
				if (!_sourceFilter.Contains(source))
					return;
				if (!_typeInfoProvider.TryGetTypeInfo(source, out var typeInfo))
					return;
				var ev = updateMsg.Data.Deserialize(typeInfo) ?? throw new JsonException("Null event.");
				HandleData(source, ev, updateMsg.MD5, true);
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
