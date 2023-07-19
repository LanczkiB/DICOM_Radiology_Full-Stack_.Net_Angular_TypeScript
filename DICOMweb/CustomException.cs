using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Web.Http.ModelBinding;

namespace DICOMweb
{
    
    public class CustomException: Exception
    {
        public string UserMessage{ get; set; }
        public CustomException(string message, string userMessage) : base(message)
        {
            this.UserMessage = userMessage;
            Logger.LogException(userMessage,message);
        }
    }
}
