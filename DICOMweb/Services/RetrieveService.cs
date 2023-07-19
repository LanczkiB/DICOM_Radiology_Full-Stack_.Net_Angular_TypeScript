using DICOMweb.Entities;
using DICOMweb.Requests;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Net;
using System.Text.Json.Nodes;

namespace DICOMweb.Services
{
    public class RetrieveService
    {
        private RetrieveRequest _retrieveRequest;
        private readonly string ROOT_FOLDER = Directory.GetCurrentDirectory() + "\\retrieve\\";
        private string _mediaType;
        private string _mediaFormat;

        public RetrieveService()
        {
            _retrieveRequest = new RetrieveRequest();
            _mediaType = string.Empty;
            _mediaFormat = ".dcm";
        }

        internal void SetMediaType(string mediaType)
        {
            _mediaType = mediaType;
        }

        internal void SetMediaFormat(string mediaFormat)
        {
            _mediaFormat = mediaFormat;
        }

        internal string GetRetrieveInstancesAddress(string serverName, string studyInstanceUID, string seriesInstanceUID, string instanceUID)
        {
            try
            {
                Uri requestUri = new(_retrieveRequest.CreateInstancesUri(serverName, studyInstanceUID + "," + seriesInstanceUID + "," + instanceUID) + "/" + _mediaType);
                HttpWebResponse response = _retrieveRequest.Execute(requestUri, new Dictionary<string, string>());
                Logger.LogStringInformation("Retrieve instance response arrived");
                string address = RetrieveSingle(response, instanceUID);
                Logger.LogStringInformation("Instance retrieve done with the address of: "+address);
                return address;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing http response stream");
            }
        }

        internal string GetRetrieveSeriesAddress(string serverName, string studyInstanceUID, string seriesInstanceUID)
        {
            try
            {
                Uri requestUri = new(_retrieveRequest.CreateSeriesUri(serverName, studyInstanceUID + "," + seriesInstanceUID) + "/" + _mediaType);

                HttpWebResponse response = _retrieveRequest.Execute(requestUri, new Dictionary<string, string>());
                Logger.LogStringInformation("Retrieve series response arrived");
                string address = RetrieveMultipart(response, studyInstanceUID, seriesInstanceUID);
                Logger.LogStringInformation("Retrieve series done with the address of: "+address);
                return address;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing http response stream");
            }
        }

        private string RetrieveMultipart(HttpWebResponse response, string studyInstanceUID, string seriesInstanceUID)
        {
            string[] contentType = response.ContentType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            IList<KeyValuePair<string, object>> parts = GetResponseParts(response.GetResponseStream(), contentType);
            Logger.LogStringInformation("HTTP GET response parsed.");
            string output = RetrieveResponseParts(parts, studyInstanceUID, seriesInstanceUID);
            response.Close();
            return output;
        }

        internal string GetRetrieveStudyAddress(string serverName, string studyInstanceUID)
        {
            try
            {
                Uri requestUri = new(_retrieveRequest.CreateStudyUri(serverName, studyInstanceUID) + "/" + _mediaType);
                HttpWebResponse response = _retrieveRequest.Execute(_retrieveRequest.CreateStudyUri(serverName, studyInstanceUID), new Dictionary<string, string>());
                Logger.LogStringInformation("Retrieve study response arrived");
                string address = RetrieveMultipart(response, studyInstanceUID, "seriesUID");
                Logger.LogStringInformation("Retrieve study done with the address of: "+address);
                return address;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing http response stream");
            }
        }

        private string RetrieveSingle(HttpWebResponse response, string instanceUID)
        {
            Stream responseStream = response.GetResponseStream();
            MemoryStream ms = new();
            responseStream.CopyTo(ms);
            byte[] data = ms.ToArray();
            var directory = Path.Combine(new string[] { System.IO.Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + "\\dicom_web_ui\\src\\assets", instanceUID + _mediaFormat });
            if (File.Exists(directory)) Logger.LogStringInformation("Instance image of " + instanceUID + " already retrieved and saved as: " + directory);
            else File.WriteAllBytes(directory, data);
            var jsonObj = new JsonObject
            {
                { "url", directory.Split("\\assets\\")[1] }
            };
            response.Close();
            return jsonObj.ToJsonString();
        }
        StreamContent? myStreamContent;

        internal IList<KeyValuePair<string, object>> GetResponseParts(Stream myStream, string[] contentType)
        {
            IList<KeyValuePair<string, object>> parts = new List<KeyValuePair<string, object>>();
            string cType = string.Empty;
            for (int i = 0; i < contentType.Length; i++)
            {

                if (contentType[i].Contains("app"))
                {
                    continue;
                }
                if (i > 0)
                {
                    cType += ";";
                }
                cType += contentType[i];
            }
            myStreamContent = new StreamContent(myStream);
            myStreamContent.Headers.Add("Content-Type", cType);

            Task<MultipartMemoryStreamProvider> task = myStreamContent.ReadAsMultipartAsync();
            task.Wait();
            MultipartMemoryStreamProvider multipart = task.Result;

            foreach (HttpContent part in multipart.Contents)
            {
                string? mediaType = part.Headers.ContentType!.MediaType;
                Task<byte[]> t = part.ReadAsByteArrayAsync();
                t.Wait();
                parts.Add(new KeyValuePair<string, object>(key: mediaType!, value: t.Result));
            }
            return parts;
        }


        internal string RetrieveResponseParts(IList<KeyValuePair<string, object>> parts, string studyUid, string seriesUid)
        {
            try
            {
                string directory = ROOT_FOLDER + studyUid;
                if (Directory.Exists(directory))
                {
                    Logger.LogStringWarning("Directory " + directory + " already exists!");
                }
                else
                {
                    Directory.CreateDirectory(directory);
                    Logger.LogStringInformation("Directory " + directory + " created!");
                }
                foreach (KeyValuePair<string, object> item in parts)
                {
                    if (seriesUid == "") seriesUid = "seriesUID";
                    string seriesDirectory = directory + "/" + seriesUid;
                    Directory.CreateDirectory(seriesDirectory);
                    if (item.Value.GetType() == typeof(string))
                    {
                        Logger.LogStringInformation("Retrieving " + seriesUid + " started.");
                        using (StreamWriter textFile = new StreamWriter(Path.Combine(new string[] { seriesDirectory, Guid.NewGuid().ToString() })))
                        {

                            textFile.Write(item.Value);
                            Logger.LogStringInformation("Retrieving " + seriesUid + " file created to folder: " + directory);
                        }
                    }
                    if (item.Value.GetType() == typeof(byte[]))
                    {
                        var new_directory = Path.Combine(new string[] { seriesDirectory, Guid.NewGuid().ToString() + _mediaFormat });
                        if (File.Exists(directory)) { }
                        else File.WriteAllBytes(new_directory, item.Value as byte[]);
                    }
                }
                return "{ \"text\": \"success\" }";
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Error occurred when retrieving " + seriesUid + " file.");
            }
        }

    }
}
