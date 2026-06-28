# Source Worker and Report Generator
A source worker manages the connection to an event source and fetches events from it. An event is a direct representation of the original information published by the event source, and it has to be converted into a report before it can be displayed in CysTerra. This conversion is done by a report generator.

The source workers and the report generators for the same event source are usually implemented in the same extension.

## Source Worker
A source worker is a class implementing the [](xref:Cryville.EEW.ISourceWorker) interface. The following is an example implementation.

```cs
using Cryville.EEW;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension {
	public class MyWorker : ISourceWorker {
		public string? GetName([NotNull] ref CultureInfo? culture) {
			// Get the name of the source worker from the resources
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		public async Task RunAsync(CancellationToken cancellationToken) {
			// Signal that the worker is connected
			Heartbeat?.Invoke(this, new());
			// Emit an object as an event
			Received?.Invoke(this, new object());
		}
	}
}
```

When this worker is started, it emits a `new object()` as an event, and then exits.

Normally a source worker does something much more complicated, usually in a loop to fetch events periodically.

```cs
public async Task RunAsync(CancellationToken cancellationToken) {
	try {
		while (true) {
			// ...
			// Fetch and parse new events if there is any
			// ...
			
			// Wait before next request
			await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken).ConfigureAwait(true);
		}
	}
	catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
		// Do nothing: Worker task cancellation requested
	}
}
```

> [!CAUTION]
> Source workers are NOT designed to fetch continuous real-time data, such as real-time intensity of seismic stations. Doing this can lead to very bad performance.
>
> A new component will be implemented to fetch these real-time data in the future.

A source worker is built with a [builder](builder.md) exported with `[Export(typeof(IBuilder<ISourceWorker>))]`.

### Built-in Base Workers
As many event sources publish events in HTTP or WebSocket, CysTerra have built in two classes to fetch events from these protocols respectively for convenience. Inherit your source worker from [](xref:Cryville.EEW.HttpPullWorker) or [](xref:Cryville.EEW.WebSocketWorker) to make use of them.

# [`HttpPullWorker`](#tab/HttpPullWorker)
[](xref:Cryville.EEW.HttpPullWorker) fetches events from the given URI by sending GET requests periodically, and then passes the response to the [](xref:Cryville.EEW.HttpPullWorker.Handle*) method if its status code is `200` (OK).

```cs
using Cryville.EEW;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension {
	public class MyHttpWorker(Uri uri) : HttpPullWorker(uri), ISourceWorker {
		public string? GetName([NotNull] ref CultureInfo? culture) {
			// Get the name of the source worker from the resources
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, new());
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			// ...
			// Deserialize the event from the response stream
			// and raise the event with Received?.Invoke()
			// ...
		}
	}
}
```

By default, [](xref:Cryville.EEW.HttpPullWorker) inspects the `max-age` directive in the `Cache-Control` response header to determine the period of requesting, and falls back to 60 seconds if the directive is not found. You can change this behavior by overriding the [](xref:Cryville.EEW.HttpPullWorker.ForceDefaultPeriod), [](xref:Cryville.EEW.HttpPullWorker.DefaultPeriod), and [](xref:Cryville.EEW.HttpPullWorker.MinimumPeriod) properties.

If you want to handle requests with status codes other than `200` (OK), override the [](xref:Cryville.EEW.HttpPullWorker.HandleRawResponse*) method. Call the base method at the end of your override.

```cs
protected override Task HandleRawResponse(HttpResponseMessage response, CancellationToken cancellationToken) {
	ThrowHelper.ThrowIfNull(response);
	if (response.StatusCode == HttpStatusCode.Unauthorized) {
		throw new SourceWorkerClientException("Authorization failed.");
	}
	return base.HandleRawResponse(response, cancellationToken);
}
```

You can modify the URI to be requested by [](xref:Cryville.EEW.HttpPullWorker) dynamically by overriding the [](xref:Cryville.EEW.HttpPullWorker.GetUri*) method.

You can send additional requests by calling the [](xref:Cryville.EEW.HttpPullWorker.TryGetAsync*) method or the [](xref:Cryville.EEW.HttpPullWorker.TrySendAsync*) method.

# [`WebSocketWorker`](#tab/WebSocketWorker)
[](xref:Cryville.EEW.WebSocketWorker) receives messages from the given URI, and passes each message to the [](xref:Cryville.EEW.WebSocketWorker.Handle*) method.

```cs
using Cryville.EEW;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyExtension {
	public class MyWebSocketWorker(Uri uri) : WebSocketWorker(uri), ISourceWorker {
		public string? GetName([NotNull] ref CultureInfo? culture) {
			// Get the name of the source worker from the resources
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, new());
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		protected override async Task Handle(Stream stream, CancellationToken cancellationToken) {
			// ...
			// Deserialize the event from the response stream
			// and raise the event with Received?.Invoke()
			// ...
		}
	}
}
```

***

### Errors
Raising [](xref:Cryville.EEW.ISourceWorker.ErrorEmitted) with an instance of [](xref:Cryville.EEW.SourceWorkerNetworkException) indicates a non-fatal network error (this error is not reportable), and the source worker is considered disconnected with the event source, until [](xref:Cryville.EEW.ISourceWorker.Heartbeat) is raised afterwards, which indicates a successful reconnection. Raising [](xref:Cryville.EEW.ISourceWorker.ErrorEmitted) with any other exceptions indicates an non-fatal non-network error.

A faulted [](xref:Cryville.EEW.ISourceWorker.RunAsync*) task (i.e. the task exits with an unhandled exception) indicates a fatal error. If it is faulted with an instance of [](xref:Cryville.EEW.SourceWorkerClientException), the error is considered to be caused by the user and is not reportable.

> [!NOTE]
> For reportable and non-reportable errors, see [Error Reporting](error-reporting.md).

## Report Generator
A report generator is a class implementing the [IGenerator](xref:Cryville.EEW.IGenerator`1)<[](xref:Cryville.EEW.Report.ReportModel)> interface or the [IContextedGenerator](xref:Cryville.EEW.IContextedGenerator`2)<[](xref:Cryville.EEW.Report.IReportGeneratorContext), [](xref:Cryville.EEW.Report.ReportModel)> interface. The following is an example implementation.

```cs
using Cryville.Common.Compat;
using Cryville.EEW;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace MyExtension {
	public class MyReportGenerator : IContextedGenerator<MyEvent, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(MyEvent e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Location = /* ... */,
				Time = /* ... */,
				TimeZone = /* ... */,
			};
			result.GroupKeys.Add(/* ... */);
			result.Properties.Add(/* ... */);

			return result;
		}
	}
}
```

A report is displayed in the report list like the following.

<div class="report" style="border: solid .25em gray; border-radius: .8em; max-width: 24em;">
	<div class="report-header" style="border-bottom: solid .25em gray; background-color: gray; text-align: center; display: flex;">
		<div style="flex: auto; border-top-left-radius: .6em;">Title | Source</div>
		<div style="flex: 0 5em; border-top-right-radius: .6em; margin-left: .25em;">#1</div>
	</div>
	<div style="display: flex;">
		<div style="background-color: gray; flex: 0 6em; text-align: center; padding-right: .25em;">
			<div>Key Prop</div>
			<div style="font-size: 1.6em;">1.0</div>
			<div>Condition</div>
		</div>
		<div style="flex: auto; padding: 0 .25em;">
			<div><span style="font-size: 1.6em;">Location</span> <span>Predicate</span></div>
			<div>2000-01-01 00:00:00 (UTC)</div>
			<div style="display: flex; gap: 2em;">
				<span>Prop1 2.0</span>
				<span>Prop2 3.0</span>
			</div>
		</div>
	</div>
</div>
<style>
.report {
	background-color: #fff;
	color: black;
}
[data-bs-theme="dark"] .report {
	background-color: #111;
	color: white;
}
.report-header > div {
	background-color: #fffc;
}
[data-bs-theme="dark"] .report-header > div {
	background-color: #111c;
}
</style>

- “Title”: [](xref:Cryville.EEW.Report.ReportModel.Title)
- “Source”: [](xref:Cryville.EEW.Report.ReportModel.Source)
- “#1”: [](xref:Cryville.EEW.Report.ReportModel.RevisionKey)
- “Location”: [](xref:Cryville.EEW.Report.ReportModel.Location)
- “Predicate”: [](xref:Cryville.EEW.Report.ReportModel.Predicate)
- “2000-01-01 00:00:00 (UTC)”: [](xref:Cryville.EEW.Report.ReportModel.Time)
- “Key Prop 1.0 Condition”: [](xref:Cryville.EEW.Report.ReportModel.Properties)[0]
- “Prop1 2.0”: [](xref:Cryville.EEW.Report.ReportModel.Properties)[1]
- “Prop2 3.0”: [](xref:Cryville.EEW.Report.ReportModel.Properties)[2]

The first item in the [](xref:Cryville.EEW.Report.ReportModel.Properties) list is considered the key property and is displayed emphasized on the left.

For more information, see the API documentation of the [](xref:Cryville.EEW.Report.ReportModel) class.

A report generator is built with a [builder](builder.md) exported with `[Export(typeof(IBuilder<IGenerator<ReportModel>>))]`.

### Report Grouping
Related reports are grouped together for easier browsing. A report revising another report is grouped into the same **report unit** with that report, and relevant report units are grouped into a **report group**.

Report grouping is based on the [](xref:Cryville.EEW.Report.ReportModel.GroupKeys) property in reports. Two reports with any matching group keys ([](xref:Cryville.EEW.Report.IReportGroupKey)) are grouped into the same report group.

[](xref:Cryville.EEW.Report.IReportUnitKey), derived from [](xref:Cryville.EEW.Report.IReportGroupKey), is for grouping reports into report units. Two reports with the same unit key are grouped into the same report unit.

### Report Validity
Invalidated reports are collapsed in CysTerra. A report is considered invalidated if either:

- Each of its unit key is covered by any unit key in another report. ([](xref:Cryville.EEW.Report.IReportUnitKey.IsCoveredBy*))
- [](xref:Cryville.EEW.Report.ReportModel.InvalidatedTime) is set and the time now is later than it.

If [](xref:Cryville.EEW.Report.ReportModel.InvalidatedTime) is set and the time now is earlier than it, the report is pinned as an ongoing event and is always displayed on the map despite whether it is selected or not, until it is invalidated.
