using System.Net;

namespace DICOMweb
{
    public abstract class DicomWebRequest
    {

        abstract internal Uri CreateStudyUri(string server, string parameters);

        abstract internal HttpWebResponse Execute(Uri requestUri,IDictionary<string, string> headers);

        abstract internal Uri CreateSeriesUri(string serverName, string parameters);

        abstract internal Uri CreateInstancesUri(string serverName, string parameters);
        
    }
}
