using DICOMweb.Entities;
using System.Net;
using System.Web;

namespace DICOMweb.Requests
{
    public class SearchRequest : DicomWebRequest
    {
        public string type = "studies";

        internal SearchRequest() { }

        internal void SetType(string type)
        {
            this.type = type;
        }

        internal Uri DecideLevel(string serverName, string? parameters)
        {
            switch (type)
            {
                case "study":
                    {
                        return CreateStudyUri(serverName, parameters);
                    }
                case "series":
                    {
                        return CreateSeriesUri(serverName, parameters);
                    }
                case "instance":
                    {
                        return CreateInstancesUri(serverName, parameters);
                    }
                default: { throw new CustomException("No additional information", "Unexpected hierarchy level"); }
            }

        }

        internal override Uri CreateStudyUri(string serverName, string parameters)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName) + "/studies";
            if (parameters != "") uri += ParsedParametersToUriString(ParameterParser(parameters));
            Logger.LogStringInformation("Search study URI created: "+uri);
            return new Uri(uri);
        }
        internal override Uri CreateSeriesUri(string serverName, string? parameters)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName);
            if (parameters != null)
            {
                Dictionary<string, string> paramDictionary = ParameterParser(parameters);
                if (paramDictionary.ContainsKey("StudyInstanceUID") || paramDictionary.ContainsKey("0020000D"))
                {
                    uri += "studies/" + paramDictionary["StudyInstanceUID"] + "/series";
                    paramDictionary.Remove("StudyInstanceUID");
                }
                else uri += "series";
                uri += ParsedParametersToUriString(paramDictionary);
            }
            Logger.LogStringInformation("Search series URI created: " + uri);
            return new Uri(uri);
        }

        internal override Uri CreateInstancesUri(string serverName, string? parameters)
        {
            UrlBuilderHelper urlBuilderHelper = new();
            string uri = urlBuilderHelper.BuildUrlFromServerData(serverName);
            if (parameters != null)
            {
                Dictionary<string, string> paramDictionary = ParameterParser(parameters);
                if (paramDictionary.ContainsKey("StudyInstanceUID") || paramDictionary.ContainsKey("0020000D"))
                {
                    uri += "studies/" + paramDictionary["StudyInstanceUID"] + "/";
                    paramDictionary.Remove("StudyInstanceUID");
                }
                if (paramDictionary.ContainsKey("SeriesInstanceUID") || paramDictionary.ContainsKey("0020000E"))
                {
                    uri += "series/" + paramDictionary["SeriesInstanceUID"] + "/";
                    paramDictionary.Remove("SeriesInstanceUID");
                }
                uri += "instances";
                if (paramDictionary.Count > 0) uri += ParsedParametersToUriString(paramDictionary);
            }
            Logger.LogStringInformation("Search instances URI created: " + uri);
            return new Uri(uri);
        }

        internal override HttpWebResponse Execute(Uri requesetUri, IDictionary<string, string> headers)
        {
            RequestExecutor request = new();
            return request.Execute(requesetUri, headers);
        }

        private static Dictionary<string, string> ParameterParser(string parameters)
        {
            Dictionary<string, string> matchingParameters = new Dictionary<string, string>();
            if (parameters != "")
            {
                var query = HttpUtility.ParseQueryString(parameters);
                if (query != null)
                {
                    foreach (var item in query.Keys)
                    {
                        if (item != null)
                            matchingParameters.Add(item.ToString()!, query.Get(item.ToString())!);
                    }
                }
            }
            return matchingParameters;
        }

        private static string ParsedParametersToUriString(Dictionary<string, string> matchingParameters)
        {
            string requestPart = "";
            foreach (var item in matchingParameters)
            {
                if (requestPart == "") requestPart += "?";
                else requestPart += "&";
                requestPart += item.Key + "=" + item.Value;
            }
            return requestPart;
        }
    }
}
