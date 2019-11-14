using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace qbService.Controllers
{
    [ApiController]
    [Route("[controller]")]
   // [Authorize]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet("aa")]
        //[Authorize]
        public IEnumerable<string> Get()
        {
            return new []{ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        }
    }
}
