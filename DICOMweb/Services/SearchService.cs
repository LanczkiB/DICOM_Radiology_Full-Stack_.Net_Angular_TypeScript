using DICOMweb.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text.Json.Nodes;
using DICOMweb.Requests;

namespace DICOMweb.Services
{
    public class SearchService
    {
        private SearchRequest _searchRequest;

        public SearchService()
        {
            _searchRequest = new SearchRequest();
        }

        internal List<JsonObject> GetSearchStudyJsonList(string serverName, string type, string? parameterList)
        {
            try
            {
                List<Resource> resourceList = GetResourceList(serverName, type, parameterList);
                List<JsonObject> studies = ParseResourceListToStudyJsonList(resourceList);
                Logger.LogStringInformation("Search study response parsed to JsonObject list");
                if (studies.Count == 0) Logger.LogStringWarning("No studies found during the execution of HTTP GET request: ");
                Logger.LogStringInformation("GET series HTTP response done with the request:");
                return studies;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing study search http response stream");
            }
        }

        private List<Resource> GetResourceList(string serverName, string type, string? parameterList)
        {
            _searchRequest.SetType(type);
            HttpWebResponse response = _searchRequest.Execute(_searchRequest.DecideLevel(serverName, parameterList), new Dictionary<string, string>());
            Logger.LogStringInformation("Http response arrived to search service.");
            return ParseResponseToResourceList(response);
        }

        internal List<JsonObject> GetSearchSeriesJsonList(string serverName, string type, string? parameterList)
        {
            try
            {
                if (type == "study") type = "series";
                List<Resource> resourceList = GetResourceList(serverName, type, parameterList);
                List<JsonObject> studies = ParseResourceListToSeriesJsonList(resourceList);
                Logger.LogStringInformation("Search series response parsed to JsonObject list");
                if (studies.Count == 0) Logger.LogStringWarning("No studies found during the execution of HTTP GET request: ");//throw new CustomException("No additional information","No series found with the given parameters!");
                Logger.LogStringInformation("GET series HTTP response done with the request:");
                return studies;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing http response stream");
            }
        }

        internal List<JsonObject> GetSearchInstancesJsonList(string serverName, string? parameterList)
        {
            try
            {
                HttpWebResponse response = _searchRequest.Execute(_searchRequest.CreateInstancesUri(serverName, parameterList), new Dictionary<string, string>());
                List<Resource> resourceList = ParseResponseToResourceList(response);
                List<JsonObject> studies = ParseResourceListToInstancesJsonList(resourceList);
                Logger.LogStringInformation("Search instances parsed to JsonObject list");
                if (studies.Count == 0) Logger.LogStringWarning("No studies found during the execution of HTTP GET request: ");//throw new CustomException("No additional information","No series found with the given parameters!");
                Logger.LogStringInformation("GET series HTTP response done with the request:");
                return studies;
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, "Problem occured while parsing http response stream");
            }
        }

        private static List<Resource> ParseResponseStream(List<JObject> responseJsonList)
        {
            List<Resource> result = new List<Resource>();
            foreach (JObject responseJsonObject in responseJsonList)
            {
                Resource newObj = new Resource();
                foreach (JProperty jsonProp in responseJsonObject.Properties())
                {
                    string number = jsonProp.Name;
                    string value = "undefined";
                    string vr = "undefined";
                    if (responseJsonObject.SelectToken(number + ".Value") != null)
                    {
                        if (responseJsonObject.SelectToken(number + ".Value[0].Alphabetic") != null)
                            value = responseJsonObject[number]!["Value"]![0]!["Alphabetic"]!.ToString();
                        else value = responseJsonObject[number]!["Value"]![0]!.ToString();
                    }
                    if (responseJsonObject.SelectToken(number + ".vr") != null)
                    {
                        vr = responseJsonObject[number]!["vr"]!.ToString();
                    }
                    newObj.SetResourceTag(number, value, vr);
                }
                result.Add(newObj);
            }
            return result;
        }

        private static List<Resource> ParseResponseToResourceList(HttpWebResponse response)
        {
            List<Resource> result = new List<Resource>();
            if (response != null)
            {
                Stream myStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(myStream);
                string text = reader.ReadToEnd();
                List<JObject>? json = JsonConvert.DeserializeObject<List<JObject>>(text);
                if (json != null)
                {
                    result = ParseResponseStream(json);
                }
                response.Close();
            }
            return result;
        }

        private static List<JsonObject> ParseResourceListToStudyJsonList(List<Resource> resourceList)
        {
            List<JsonObject> studies = new();
            if (resourceList != null)
            {
                foreach (Resource resource in resourceList)
                {

                    if (resource.StudyInstanceUID.Value == "Undefined")
                        break;
                    var jsonObj = new JsonObject
                    {
                        { "patientName", resource.PatientName.Value },
                        { "studyInstanceUID", resource.StudyInstanceUID.Value },
                        { "studyDate", resource.StudyDate.Value },
                        { "studyTime", resource.StudyTime.Value },
                        { "accessionNumber", resource.AccessionNumber.Value },
                        { "instanceAvailability", resource.InstanceAvailability.Value },
                        { "modalitiesInStudy", resource.ModalitiesInStudy.Value },
                        { "referringPhysiciansName", resource.ReferringPhisicianName.Value },
                        { "timezone", resource.Timezone.Value },
                        { "retrieveURL", resource.RetrieveURL.Value },
                        { "patientsName", resource.PatientName.Value },
                        { "patientID", resource.PatientID.Value },
                        { "patientBirthDate", resource.PatientBirthDate.Value },
                        { "patientsSex", resource.PatientsSex.Value },
                        { "studyID", resource.StudyID.Value },
                        { "numberOfSeries", resource.NumberOfStudyRelatedSeries.Value },
                        { "numberOfInstances", resource.NumberOfStudyRelatedInstances.Value }
                    };

                    bool contains = false;
                    foreach (JsonObject obj in studies)
                    {
                        //?exception
                        JsonNode node;
                        if (obj.TryGetPropertyValue("studyInstanceUID", out node!))
                            if (node.ToString() == resource.StudyInstanceUID.Value) contains = true;
                    }
                    if (!contains) studies.Add(jsonObj);
                }
            }
            return studies;
        }

        private static List<JsonObject> ParseResourceListToSeriesJsonList(List<Resource> resourceList)
        {
            List<JsonObject> series = new();
            if (resourceList != null)
            {
                foreach (Resource resource in resourceList)
                {
                    JsonObject jsonObj = new()
                    {
                        { "seriesInstanceUID", resource.SeriesInstanceUID.Value },
                        { "studyUID", resource.StudyInstanceUID.Value },
                        { "seriesNumber", resource.SeriesNumber.Value },
                        { "modality", resource.Modality.Value },
                        { "seriesDescription", resource.SeriesDescription.Value },
                        { "retrieveURL", resource.RetrieveURL.Value },
                        { "numberOfSeriesInstances", resource.NumberOfStudyRelatedInstances.Value },
                        { "stepStartDate", resource.StepStartDate.Value },
                        { "stepStartTime", resource.StepStartTime.Value },
                        { "requestAttributesSequence", resource.RequestAttributesSequence.Value }
                       };
                    series.Add(jsonObj);
                }
            }
            return series;
        }

        private static List<JsonObject> ParseResourceListToInstancesJsonList(List<Resource> resourceList)
        {
            List<JsonObject> studies = new();
            if (resourceList != null)
            {
                foreach (Resource resource in resourceList)
                {
                    var jsonObj = new JsonObject
                    {
                        { "SOPInstanceUID", resource.SopInstanceUID.Value },
                        { "timezone", resource.Timezone.Value },
                        { "retrieveURL", resource.RetrieveURL.Value },
                        { "instanceNumber", resource.InstanceNumber.Value },
                        { "rows", resource.Rows.Value },
                        { "columns", resource.Columns.Value },
                        { "bitsAllocated", resource.BitsAllocated.Value },
                        { "numberOfFrames", resource.NumberOfFrames.Value },
                    };
                    studies.Add(jsonObj);
                }
            }
            return studies;
        }

    }
}
