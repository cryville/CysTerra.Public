using Cryville.Common.Compat;
using Cryville.EEW.CWAOpenData.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Cryville.EEW.CWAOpenData {
	public class CWAReportWorker<T>(Uri uri, string token, double baseReportDelay = 10, double maxReportDelay = 1440) : HttpPullWorker(uri), ISourceWorker<T> where T : CWAReport {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(typeof(T).Name, ref culture);

		public event Handler<T?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		protected override Task HandleRawResponse(HttpResponseMessage response, CancellationToken cancellationToken) {
			ThrowHelper.ThrowIfNull(response);
			if (response.StatusCode == HttpStatusCode.Unauthorized) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				throw new SourceWorkerClientException(res.GetStringRequired("ErrorUnauthorized"));
			}
			return base.HandleRawResponse(response, cancellationToken);
		}

		bool _init;
		DateTime? _lastDate;
		readonly HashSet<(string, string)> _history = [];
		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			ThrowHelper.ThrowIfNull(headers);
			if (headers.Date is DateTimeOffset date) {
				_lastDate = TimeZoneInfo.ConvertTimeFromUtc(date.UtcDateTime, Local.TimeZone);
			}
			var result = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.CWAResult, cancellationToken).ConfigureAwait(true) ?? throw new InvalidOperationException("Empty records.");
			if (result.Records is not CWARecords records) return;
			T[] list;
			if (typeof(T) == typeof(Tsunami)) list = Unsafe.As<T[]>(records.Tsunami);
			else if (typeof(T) == typeof(Earthquake)) list = Unsafe.As<T[]>(records.Earthquakes);
			else throw new InvalidOperationException("Invalid record type.");
			if (list is null) return;
			if (_init) {
				foreach (var entry in list.Reverse()) {
					if (!_history.Add((entry.ReportContent, entry.Web))) continue;
					Received?.Invoke(this, entry);
				}
				if (_currentDelay == _maxReportDelay) {
					_history.RemoveWhere(e => !list.Any(r => (r.ReportContent, r.Web) == e));
				}
			}
			else {
				_init = true;
				foreach (var entry in list) {
					_history.Add((entry.ReportContent, entry.Web));
				}
			}
		}

		readonly Dictionary<string, string> _params = new() {
			{ "Authorization", token },
		};
		protected override Uri GetUri() {
			if (_lastDate is DateTime date && _currentDelay != _maxReportDelay) {
				_params["timeFrom"] = (date - TimeSpan.FromMinutes(_currentDelay)).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
			}
			else {
				_params.Remove("timeFrom");
			}
			return new(BaseUri + "?" + string.Join("&", _params.Select(i => HttpUtility.UrlEncode(i.Key) + "=" + HttpUtility.UrlEncode(i.Value))));
		}

		readonly double _maxReportDelay = maxReportDelay;
		readonly DynamicDelay _delay = new(baseReportDelay, maxReportDelay);
		double _currentDelay = maxReportDelay;
		protected override Task AfterHandled(CancellationToken cancellationToken) {
			_currentDelay = _delay.IncrementPhase(1);
			return Task.CompletedTask;
		}
	}
}
