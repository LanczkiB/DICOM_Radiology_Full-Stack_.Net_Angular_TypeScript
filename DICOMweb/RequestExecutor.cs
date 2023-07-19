using DICOMweb.Entities;
using System.Net;

namespace DICOMweb
{
    public class RequestExecutor
    {
        internal HttpWebRequest? myRequest;

        internal HttpWebResponse Execute(Uri address, IDictionary<string, string> headers)
        {

            myRequest = (HttpWebRequest)WebRequest.Create(address);
            myRequest.Method = "GET";
            foreach (KeyValuePair<string, string> header in headers)
            {
                myRequest.Headers.Add(header.Key, header.Value);
            }


            Logger.LogStringInformation("HTTP GET request sent.");
            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();
            Logger.LogStringInformation("HTTP GET response arrived.");
            return response;
        }

    }
}
