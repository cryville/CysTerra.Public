using Cryville.EEW.ComponentModel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.USGS {
	public abstract class USGSWorker : HttpPullWorker, ISourceWorker, IPropertiesHolder {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		USGSFeedType m_feed;
		[LocalizableDisplayName("PNFeed")]
		public USGSFeedType Feed {
			get => m_feed;
			[MemberNotNull(nameof(_uri))]
			set {
				if (m_feed == value && _uri != null)
					return;
				m_feed = value;
				var relativeUri = new Uri(string.Format(CultureInfo.InvariantCulture, "/earthquakes/feed/v1.0/summary/{0}{1}", value switch {
					USGSFeedType.SignificantHour => "significant_hour",
					USGSFeedType.M45Hour => "4.5_hour",
					USGSFeedType.M25Hour => "2.5_hour",
					USGSFeedType.M10Hour => "1.0_hour",
					USGSFeedType.AllHour => "all_hour",
					USGSFeedType.SignificantDay => "significant_day",
					USGSFeedType.M45Day => "4.5_day",
					USGSFeedType.M25Day => "2.5_day",
					USGSFeedType.M10Day => "1.0_day",
					USGSFeedType.AllDay => "all_day",
					USGSFeedType.SignificantWeek => "significant_week",
					USGSFeedType.M45Week => "4.5_week",
					USGSFeedType.M25Week => "2.5_week",
					USGSFeedType.M10Week => "1.0_week",
					USGSFeedType.AllWeek => "all_week",
					USGSFeedType.SignificantMonth => "significant_month",
					USGSFeedType.M45Month => "4.5_month",
					USGSFeedType.M25Month => "2.5_month",
					USGSFeedType.M10Month => "1.0_month",
					USGSFeedType.AllMonth => "all_month",
					_ => "significant_week",
				}, _extension), UriKind.Relative);
				_uri = new(BaseUri, relativeUri);
				InvalidateHistory();
			}
		}

		Uri _uri;
		string _extension;
		protected override Uri GetUri() => _uri;
		[Obsolete("Use the Feed property.")]
		public void SetFeedRelativeUri(Uri feedUri) {
			_uri = new(BaseUri, feedUri);
			InvalidateHistory();
		}
		protected abstract void InvalidateHistory();

		protected USGSWorker(Uri uri, string extension) : base(uri) {
			_extension = extension;
			Feed = USGSFeedType.SignificantWeek;
		}

		public abstract event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
	}
}
