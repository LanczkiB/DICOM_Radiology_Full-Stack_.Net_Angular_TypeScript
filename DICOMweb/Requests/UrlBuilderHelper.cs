using DICOMweb.Configuration;
using DICOMweb.Entities;
using System.Web;

namespace DICOMweb.Requests
{
    public class UrlBuilderHelper
    {
        public UrlBuilderHelper() { }

        internal string BuildUrlFromServerData(string serverName)
        {
            NodeConfiguration nodeConfiguration = new();
            Server server = nodeConfiguration.GetServer(serverName);
            return "http" + "://" + server.GetHost() + ":" + server.GetPort() + server.GetRoot();
        }

    }
}
