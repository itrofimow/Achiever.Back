using System;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Route("api/[controller]")]
    public class TimeController : AchieverController
    {
        [HttpGet]
        public CurrentTimeResponse Get()
        {
            return new CurrentTimeResponse
            {
                CurrentTime = DateTime.Now.ToString("")
            };
        }
    }

    public class CurrentTimeResponse
    {
        public string CurrentTime { get; set; }
    }
}