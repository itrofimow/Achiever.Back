using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    public class AchieverController : ControllerBase
    {
        protected WebUser WebUser => GetUser();

        private WebUser GetUser()
        {
            var userId = this.User.FindFirst(CustomClaimTypes.Id)?.Value;
            var nickname = this.User.FindFirst(CustomClaimTypes.Nickname)?.Value;

            return new WebUser
            {
                Id = userId,
                Nickname = nickname
            };
        }
    }

    public class WebUser
    {
        public string Id { get; set; }

        public string Nickname { get; set; }
    }
}