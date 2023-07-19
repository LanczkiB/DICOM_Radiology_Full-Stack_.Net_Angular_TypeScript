using DICOMweb.Entities;
using DICOMweb.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Text.Json.Nodes;
using System.Web;

namespace DICOMweb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private SearchService _searchService;

        public SearchController()
        {
            _searchService= new SearchService();
        }

        [HttpGet("/{serverName}/Search/Study")]
        public IActionResult AllStudySearch(string serverName)
        {
            try
            {
                Logger.LogStringInformation("GET HTTP Search all studies request arrived to backend server");
                var filteredStudies = _searchService.GetSearchStudyJsonList(serverName, "study", "");
                return Ok(filteredStudies);
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
                string userMessage = "Exception occurred during searching for all studies ";
                ModelState.AddModelError("userMessage", userMessage);
                Logger.LogException(userMessage, ex.Message);
                return BadRequest(ModelState);
            }
        }


        [HttpGet("/{serverName}/Search/Study/{type}/{parameterList}")]
        public IActionResult StudySearch(string serverName, string type,string parameterList)
        {
            try
            {
                Logger.LogStringInformation("GET HTTP Search study request received with the parameters of: " + parameterList);
                var filteredStudies=_searchService.GetSearchStudyJsonList(serverName,type,parameterList);
                return Ok(filteredStudies);
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
                string userMessage = "Exception occurred during searching for studies with the parameters of: "+parameterList;
                ModelState.AddModelError("userMessage", userMessage);
                Logger.LogException(userMessage, ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("/{serverName}/Search/Series/{type}/{parameterList}")]
        public IActionResult SeriesSearch(string serverName,string type, string parameterList)
        {
            try
            {
                Logger.LogStringInformation("GET HTTP Search series request received with the parameters of: " + parameterList);
                var filteredSeries = _searchService.GetSearchSeriesJsonList(serverName,type,parameterList);
                return Ok(filteredSeries);
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
                string userMessage = "Exception occurred during searching for series with the parameters of: "+parameterList;
                ModelState.AddModelError("userMessage", userMessage);
                Logger.LogException(userMessage, ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpGet("/{serverName}/Search/Instances/{parameterList}")]
        public IActionResult InstancesSearch(string serverName, string parameterList)
        {
            try
            {
                Logger.LogStringInformation("GET HTTP Search instances request received with the parameters of: " + parameterList);
                var filteredStudies = _searchService.GetSearchInstancesJsonList(serverName, parameterList);
                return Ok(filteredStudies);
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
                string userMessage = "Exception occurred during searching for instances with the parameters of: "+parameterList;
                ModelState.AddModelError("userMessage", userMessage);
                Logger.LogException(userMessage, ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
