using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace PlayGround.ChatService.Controllers
{
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        
        public BaseController(IConfiguration configuration) {
            _configuration = configuration;
        
        }
        protected string GetConfigValue(string key)
        {
           var value =  _configuration[key];
            if(value == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return value;
        }
    }
}
