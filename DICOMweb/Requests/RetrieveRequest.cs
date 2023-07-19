using System.Net;

namespace DICOMweb.Requests
{
    public class RetrieveRequest : DicomWebRequest
    {
        internal override Uri CreateInstancesUri(string serverName, string parameters)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string[] uids = parameters.Split(',');
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName) + "studies/" + uids[0] + "/series/" + uids[1] + "/instances/" + uids[2];
            return new Uri(uri);
        }

        internal override Uri CreateSeriesUri(string serverName, string parameters)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string[] uids = parameters.Split(',');
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName) + "studies/" + uids[0] + "/series/" + uids[1];
            return new Uri(uri);
        }

        internal override Uri CreateStudyUri(string serverName, string studyInstanceUID)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName) + "studies/" + studyInstanceUID;
            return new Uri(uri);
        }

        internal override HttpWebResponse Execute(Uri uri, IDictionary<string, string> headers)
        {
            RequestExecutor request = new();
            return request.Execute(uri, headers);
        }
    }
}
