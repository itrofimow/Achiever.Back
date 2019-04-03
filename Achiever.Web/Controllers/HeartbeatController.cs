using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Route("api/[controller]")]
    public class HeartbeatController : AchieverController
    {
        [HttpPut]
        public Task Heartbeat()
        {
            return Task.CompletedTask;
        }
    }
}