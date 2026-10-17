using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.USGS {
	public interface IUSGSGeoJSONFeedWorker {
		Task<HttpResponseMessage?> TryGetAsync(Uri uri, CancellationToken cancellationToken);
		void OnReceived(object e);
		void OnError(Exception ex);
	}
}