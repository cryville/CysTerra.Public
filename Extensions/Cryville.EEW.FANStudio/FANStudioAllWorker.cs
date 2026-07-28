using Cryville.EEW.ComponentModel;
using Cryville.EEW.FANStudio.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
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
		protected virtual IEnumerable<FANStudioSource> EnumerateFilter() => m_filter.Concat(m_authorizedFilter.Cast<FANStudioSource>());
		protected void MapFilter() {
			_sourceFilter = [.. EnumerateFilter().Select(i => _typeInfoProvider.TryGetSource(i, out string? source) ? source : "")];
		}
		ISet<FANStudioSource> m_filter = (HashSet<FANStudioSource>)[];
		HashSet<string> _sourceFilter = [];
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IEnumerable<string> filter) => _sourceFilter = [.. filter];

		[LocalizableDisplayName("PNAuthorizedFilter")]
		[LocalizableDescription("PDAuthorizedFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<FANStudioAuthorizedSource> AuthorizedFilter {
			get => m_authorizedFilter;
			set {
				m_authorizedFilter = value;
				MapFilter();
			}
		}
		ISet<FANStudioAuthorizedSource> m_authorizedFilter = (HashSet<FANStudioAuthorizedSource>)[];

#if NET5_0_OR_GREATER
		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
#endif
		public IEnumerable<PropertyDescriptor> GetProperties() {
			var props = TypeDescriptor.GetProperties(this).OfType<PropertyDescriptor>().Where(p => p.IsBrowsable && !p.IsReadOnly);
			if (string.IsNullOrEmpty(_authKey))
				props = props.Where(p => p.Name != nameof(AuthorizedFilter));
			return props;
		}

		readonly FANStudioSourceTypeInfoProvider _typeInfoProvider;

		readonly string? _authKey;

		public FANStudioAllWorker(Uri uri, string? authKey, FANStudioSourceTypeInfoProvider typeInfoProvider) : base(uri) {
			_authKey = authKey;
			_typeInfoProvider = typeInfoProvider;
		}

		protected override async Task OnConnected(CancellationToken cancellationToken) {
			await base.OnConnected(cancellationToken).ConfigureAwait(true);
			if (string.IsNullOrEmpty(_authKey))
				return;
			using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, new FANStudioAuthMessage(_authKey), SerializerContext.Default.FANStudioAuthMessage, cancellationToken).ConfigureAwait(true);
			stream.Position = 0;
			await SendAsync(stream, WebSocketMessageType.Text, cancellationToken).ConfigureAwait(true);
		}

		readonly Dictionary<string, (HashSet<string>, Queue<string>)> _history = [];

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
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
			else if (e is FANStudioErrorMessage errorMsg) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				ErrorEmitted?.Invoke(this, new InvalidOperationException(string.Format(SharedCultures.CurrentCulture, res.GetStringRequired("ErrorServer"), errorMsg.Message)));
			}
			else if (e is FANStudioAuthFailureMessage authFailureMessage) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				throw new SourceWorkerClientException(string.Format(SharedCultures.CurrentCulture, res.GetStringRequired("ErrorAuthFailure"), authFailureMessage.Message));
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
