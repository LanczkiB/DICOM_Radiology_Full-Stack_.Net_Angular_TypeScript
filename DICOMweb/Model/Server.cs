using Newtonsoft.Json.Linq;

namespace DICOMweb.Entities
{
    public class Server
    {
        private string Port { get; set; }
        private string Host { get; set; }
        private string Root { get; set; }
        private bool Secure { get; set; }
        private string Name { get; set; }

        public Server(JObject jsonObj)
        {
            string tokenName;
            if (jsonObj.Properties().Count() == 0) throw new CustomException("Empty server configuration file!", "No servers configured!");
            else tokenName = jsonObj.Properties().First().Name;
            var tokenPort = jsonObj.SelectToken(tokenName+".HttpPort");
            var tokenHost = jsonObj.SelectToken(tokenName + ".Host");
            var tokenRoot = jsonObj.SelectToken(tokenName + ".Root");
            var tokenSecure = jsonObj.SelectToken(tokenName + ".Secure");
            //eliminating null reference exception
            if (tokenName == null || tokenPort == null || tokenHost == null || tokenRoot == null || tokenSecure == null) { 
                throw new CustomException("Config.json should contain HttpPort, Host, Root, Secure tokens","Not appropriate server configuration file!");
            }
            else
            {
                Name = tokenName.ToString();
                Port = tokenPort.ToString();
                Host = tokenHost.ToString();
                Root = tokenRoot.ToString();
                Secure = (bool)tokenSecure;
            }
        }


        internal string GetPort()
        {
            return Port;
        }

        internal string GetHost()
        {
            return Host;
        }

        internal string GetRoot()
        {
            return Root;
        }

        internal string GetSecured()
        {
            return Secure? "http":"https";
        }

        internal string GetName()
        {
            return Name;
        }
    }
}
