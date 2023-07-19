using DICOMweb.Entities;
using DICOMweb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using System.Web;

namespace DICOMweb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RetrieveController : Controller
    {
        private RetrieveService _retrieveService;
        public RetrieveController()
        {
            _retrieveService= new RetrieveService();
        }

        
        [HttpGet("/{serverName}/Retrieve/Study/{id}")]
        public IActionResult RetrieveDicomStudy(string serverName,string id)
        {
            try
            {
                Logger.LogStringInformation("Retrieving study "+id+" HTTP request received!");
                var address = _retrieveService.GetRetrieveStudyAddress(serverName,id);
                return Ok(address);
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return CustomBadRequest(ex, "Exception occurred during HTTP GET retrieving study " + id);
            }
        }

        [HttpGet("/{serverName}/Retrieve/Series/{studyInstanceUID}/{seriesInstanceUID}")]
        public IActionResult RetrieveDicomSeries(string serverName, string studyInstanceUID, string seriesInstanceUID)
        {
            try
            {
                Logger.LogStringInformation("Retrieving series " + seriesInstanceUID + " HTTP request received!");
                var address = _retrieveService.GetRetrieveSeriesAddress(serverName, studyInstanceUID, seriesInstanceUID);
                return Ok(address);
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return CustomBadRequest(ex, "Exception occurred during HTTP GET retrieving series" + seriesInstanceUID);
            }
        }

        [HttpGet("/{serverName}/Retrieve/Instances/{studyInstanceUID}/{seriesInstanceUID}/{instanceUID}")]
        public IActionResult RetrieveDicomInstances(string serverName, string studyInstanceUID, string seriesInstanceUID, string instanceUID)
        {
            try
            {
                Logger.LogStringInformation("Retrieving instance " + instanceUID + " HTTP request received!");
                var address = _retrieveService.GetRetrieveInstancesAddress(serverName, studyInstanceUID, seriesInstanceUID, instanceUID);
                return Ok(address);
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return CustomBadRequest(ex, "Exception occurred during HTTP GET retrieving instance " + instanceUID);
            }
        }

        [HttpGet("/{serverName}/Retrieve/Rendered/Instances/{studyInstanceUID}/{seriesInstanceUID}/{instanceUID}/{mediaFormat}")]
        public IActionResult RetrieveRenderedInstances(string serverName, string studyInstanceUID, string seriesInstanceUID, string instanceUID, string mediaFormat)
        {
            try
            {
                Logger.LogStringInformation("Retrieving instance " + instanceUID + " in "+mediaFormat+" HTTP request received!");
                _retrieveService.SetMediaFormat(mediaFormat);
                _retrieveService.SetMediaType("/rendered");
                var address = _retrieveService.GetRetrieveInstancesAddress(serverName, studyInstanceUID, seriesInstanceUID, instanceUID);
                return Ok(address);
            }
            catch (CustomException ex)
            {
                ModelState.AddModelError("message", ex.Message);
                ModelState.AddModelError("userMessage", ex.UserMessage);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return CustomBadRequest(ex, "Exception occurred during HTTP GET retrieving " + mediaFormat + " instance " + instanceUID);
            }
        }
        private BadRequestObjectResult CustomBadRequest(Exception ex, string userMessage)
        {
            ModelState.AddModelError("message", ex.Message);
            ModelState.AddModelError("userMessage", userMessage);
            Logger.LogException(userMessage, ex.Message);
            return BadRequest(ModelState);
        }

    }
}
