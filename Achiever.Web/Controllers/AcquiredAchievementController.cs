using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AcquiredAchievementController : ControllerBase
    {
        private readonly ICurrentUser _currentUser;
        private readonly IAcquiredAchievementService _acquiredAchievementService;

        public AcquiredAchievementController(
            ICurrentUser currentUser,
            IAcquiredAchievementService acquiredAchievementService
        )
        {
            
            _currentUser = currentUser;
            _acquiredAchievementService = acquiredAchievementService;
        }
        
        [HttpGet]
        [Route("my/category/{categoryId}")]
        public async Task<GetAllAchievementsResponse> GetMyByCategory(string categoryId)
        {
            var data = await _acquiredAchievementService.GetByOwnerAndCategory(_currentUser.UserId, categoryId);

            return new GetAllAchievementsResponse
            {
                Achievements = data
            };
        }

        [HttpGet]
        [Route("my/followings/{achievementId}")]
        public async Task<AllUsersDto> GetFollowingsWhoHave(string achievementId)
        {
            var data = await _acquiredAchievementService.GetFollowingsWhoHave(achievementId,
                _currentUser.UserId);

            return new AllUsersDto
            {
                AllUsers = data.Select(x => new UserDto
                {
                    User = x,
                    Following = true
                }).ToList()
            };
        }
    }
}