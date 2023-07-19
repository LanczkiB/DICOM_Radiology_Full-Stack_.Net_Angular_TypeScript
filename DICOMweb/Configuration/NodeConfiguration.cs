using DICOMweb.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web.Http.ModelBinding;

namespace DICOMweb.Configuration
{
    public class NodeConfiguration
    {
        private Dictionary<string, Server> servers;

        public NodeConfiguration()
        {
            servers = new Dictionary<string, Server>();
            string route = Directory.GetCurrentDirectory() + "\\config.json";
            Console.WriteLine(route);
            try
            {
                using (StreamReader r = new(route))
                {
                    string json = r.ReadToEnd();
                    //handling if the config.json is empty
                    if (json == "") throw new CustomException("Not appropriate server configuration file", "Empty server configuration file!");
                    List<JObject>? jsonObjList = JsonConvert.DeserializeObject<List<JObject>>(json);
                    //if there are correct jsonObjects in the list
                    if (jsonObjList!.Count > 0)
                    {
                        foreach (JObject jsonObj in jsonObjList)
                        {
                            if (jsonObj.Property("Trace") != null)
                            {
                                Logger.SetTrace((bool)jsonObj.SelectToken("Trace.TraceIsOn")!);
                            }
                            else
                            {
                                Server newServer = new Server(jsonObj);
                                servers.Add(newServer.GetName(), newServer);
                            }
                        }
                    }
                    else throw new CustomException("No servers have been configured!", "Not approprtiate server configuration file!");
                }
            }
            catch (FileNotFoundException ex) { throw new CustomException(ex.Message, "No server configuration file found!"); }
            catch (JsonReaderException ex) { throw new CustomException(ex.Message, "Not appropriate server configuration file!"); }
            catch (JsonSerializationException ex){ throw new CustomException(ex.Message, "Not appropriate server configuration file!");}
            catch (Exception ex) { throw new CustomException(ex.Message, "Problem occured while reading server configuration file!"); }
        }

        internal List<string> GetServerList()
        {
            List<string> servers = new();
            foreach (var server in this.servers!)
            {
                servers.Add(server.Key);
            }
            return servers;
        }

        internal Server GetServer(string name)
        {
            return servers![name];
        }
    }
}
