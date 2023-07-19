using DICOMweb.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace DICOMweb.Configuration
{
    public class AttributeConfiguration : Controller
    {
        //Store different level search attributes
        private Dictionary<string, bool> StudyAttributes;
        private Dictionary<string, bool> SeriesAttributes;

        public AttributeConfiguration()
        {
            StudyAttributes = new Dictionary<string, bool>();
            SeriesAttributes = new Dictionary<string, bool>();
            string route = Directory.GetCurrentDirectory() + "\\attributes.json";
            try
            {
                using (StreamReader r = new(route))
                {
                    string attributesFileContent = r.ReadToEnd();
                    SetAttributes(attributesFileContent, "Study", StudyAttributes);
                    SetAttributes(attributesFileContent, "Series", SeriesAttributes);
                    if (SeriesAttributes.Count == 0 && StudyAttributes.Count == 0) Logger.LogStringWarning("No attributes declared in the attributes.config file.");
                }
            }
            catch (FileNotFoundException ex) { throw new CustomException(ex.Message, "No attribute configuration file found!"); }
            catch (Exception ex) { throw new CustomException(ex.Message, "Problem occured while reading attribute configuration file!"); }
        }

        private static void SetAttributes(string fileContent, string key, Dictionary<string, bool> dict)
        {
            JObject jsonObj = JsonConvert.DeserializeObject<JObject>(fileContent)!;
            if (jsonObj.Property(key) != null)
            {
                JObject selectedType = new(jsonObj.Property(key)!.Values());
                foreach (var jobj in selectedType.Properties())
                {
                    Resource R = new();
                    bool contains = false;
                    foreach  (PropertyInfo info in R.GetType().GetRuntimeProperties())
                    {
                        if (info.Name == jobj.Name)
                        {
                            dict.Add(jobj.Name, (bool)jobj.Value);
                            contains = true;
                            break;
                        }
                    }
                    if(!contains) throw new CustomException("Error occured during attribute configuration!", "Validation failed for attribute: " + jobj.Name);

                }
            }

        }

        internal Dictionary<string, bool> GetStudyAttributes()
        {
            return StudyAttributes;
        }

        internal Dictionary<string, bool> GetSeriesAttributes()
        {
            return SeriesAttributes;
        }
    }
}
