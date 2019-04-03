using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AllUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public AllUsersController(
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<AllUsersDto> GetAll()
        {
            var allUsers = await _userRepository.GetAll();

            return new AllUsersDto
            {
                AllUsers = allUsers.Select(x => new UserDto
                {
                    User = x,
                    Following = _currentUser.User.FollowingIds.Contains(x.Id)
                }).ToList()
            };
        }
    }

    public class AllUsersDto
    {
        public List<UserDto> AllUsers { get; set; }
    }

    public class UserDto
    {
        public User User { get; set; }

        public bool Following { get; set; }
    }
}