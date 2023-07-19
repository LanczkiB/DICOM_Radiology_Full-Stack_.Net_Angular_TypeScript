using DICOMweb.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace DICOMweb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : Controller
    {

        [HttpGet("/Server")]
        public IActionResult GetServer()
        {
            Logger.CreateNew();
            try
            {
                NodeConfiguration _configNode = new NodeConfiguration();
                Logger.LogSuccessfulServerConfiguration(_configNode.GetServerList());
                return Ok(_configNode.GetServerList().ToArray());
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Logger.LogException("Not handled exception occured!", ex.Message);
                return BadRequest("Not handled exception occured!"+ex.InnerException);
            }
        }

        [HttpGet("/Attributes")]
        public IActionResult GetStudyAttribute()
        {
            try
            {
                AttributeConfiguration _configAttribute = new AttributeConfiguration();
                Dictionary<string, bool> studyattributes = _configAttribute.GetStudyAttributes();
                Dictionary<string, bool> seriesattributes = _configAttribute.GetSeriesAttributes();
                var result = Enumerable.Concat(DictionaryToJsonList(studyattributes, "study"), DictionaryToJsonList(seriesattributes, "series"));
                Logger.LogStringInformation("Successful attributes.config configuration.");
                return Ok(result);
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", "Not handled exception occured!");
                Logger.LogException("Not handled exception occured!", ex.Message);
                return BadRequest(ModelState);
            }
        }

        private static List<JsonObject> DictionaryToJsonList(Dictionary<string, bool> attributes, string type)
        {
            List<JsonObject> output = new List<JsonObject>();
            if (attributes != null)
            {
                foreach (var attrib in attributes)
                {
                    var jsonObj = new JsonObject
                    {
                        { "attrName", attrib.Key },
                        { "attrValue", "" },
                        { "type", type }
                    };
                    output.Add(jsonObj);
                }
            }
            return output;
        }
    }
}
