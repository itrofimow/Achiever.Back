using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ICurrentUser _currentUser;

        public SearchController(
            ISearchService searchService,
            ICurrentUser currentUser)
        {
            _searchService = searchService;
            _currentUser = currentUser;
        }

        [HttpPost]
        public async Task<SearchResultDto> Search([FromBody] SearchRequestDto request)
        {
            var usersTask = _searchService.SearchUsers(request.Query);
            var achievementsTask = _searchService.SearchAchievements(request.Query);

            await Task.WhenAll(usersTask, achievementsTask);
            
            return new SearchResultDto
            {
                Achievements = achievementsTask.Result,
                Users = usersTask.Result.Select(x => new UserDto
                {
                    Following = _currentUser.User.FollowingIds.Contains(x.Id),
                    User = x
                }).ToList()
            };
        }
    }

    public class SearchRequestDto
    {
        public string Query { get; set; }
    }

    public class SearchResultDto
    {
        public List<UserDto> Users { get; set; }

        public List<Achievement> Achievements { get; set; }
    }
}