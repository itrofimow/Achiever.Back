using System.Threading.Tasks;
using Achiever.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SocialInteractionsController : ControllerBase
    {
        private readonly ISocialInteractionsService _socialInteractionsService;
        private readonly ICurrentUser _currentUser;

        public SocialInteractionsController(
            ISocialInteractionsService socialInteractionsService,
            ICurrentUser currentUser)
        {
            _socialInteractionsService = socialInteractionsService;
            _currentUser = currentUser;
        }

        [HttpPut]
        public Task Follow([FromBody]SocialInteractionRequest request)
        {
            return _socialInteractionsService.Follow(_currentUser.UserId, request.TargetId);
        }

        [HttpDelete]
        public Task Unfollow([FromBody]SocialInteractionRequest request)
        {
            return _socialInteractionsService.Unfollow(_currentUser.UserId, request.TargetId);
        }
    }

    public class SocialInteractionRequest
    {
        public string TargetId { get; set; }
    }
}