using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.Models;
using Achiever.Core.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IFileService _fileService;
        private readonly ICurrentUser _currentUser;

        public UserController(
            IUserService userService,
            ITokenService tokenService,
            IFileService fileService,
            ICurrentUser currentUser)
        {
            _userService = userService;
            _tokenService = tokenService;
            _fileService = fileService;
            _currentUser = currentUser;
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public async Task<UserWithToken> Authenticate([FromBody]AuthRequest request)
        {
            var user = await _userService.Authenticate(request.Name, request.Password, request.FirebaseToken);

            return new UserWithToken
            {
                Token = _tokenService.CreateToken(user),
                User = user
            };
        }

        [AllowAnonymous]
        [Route("signup")]
        [HttpPost]
        public async Task<UserWithToken> Signup([FromBody] SignupRequest request)
        {
            await _userService.Create(request.Nickname, request.Password);
            var user = await _userService.Authenticate(request.Nickname, request.Password, request.FirebaseToken);

            return new UserWithToken
            {
                Token = _tokenService.CreateToken(user),
                User = user
            };
        }

        [HttpGet]
        [Route("current")]
        public Task<User> GetCurrent()
        {
            return _userService.GetById(_currentUser.UserId);
        }

        [HttpGet]
        [Route("current/notifications")]
        public async Task<NotificationsList> GetNotifications()
        {
            var notifications = await _userService.GetNotifications(_currentUser.UserId);

            return new NotificationsList
            {
                Notifications = notifications
            };
        }

        [HttpGet]
        [Route("{userId}")]
        public Task<User> GetById(string userId)
        {
            return _userService.GetById(userId);
        }
        
        [HttpPost]
        [Route("current")]
        [Consumes("multipart/form-data")]
        public async Task<User> Update([FromForm] UpdateProfileRequest request)
        {
            var model = new User
            {
                Nickname = request.Nickname,
                About = request.About,
                DisplayName = request.DisplayName
            };
            
            if (request.Image != null)
            {
                model.ProfileImagePath = await _fileService.SaveFile(
                    request.Image.OpenReadStream());
            }

            return await _userService.Update(_currentUser.UserId, model);
        }

        [HttpGet]
        [Route("countEntries/{userId}")]
        public async Task<EntriesCountDto> CountFeedEntries(string userId)
        {
            var count = await _userService.CountFeedEntries(userId);
            
            return new EntriesCountDto
            {
                Count = count
            };
        }

        [HttpGet]
        [Route("getFollowers/{userId}")]
        public async Task<AllUsersDto> GetFollowers(string userId)
        {
            var result = await _userService.GetFollowers(userId);

            return new AllUsersDto
            {
                AllUsers = result.Select(x => new UserDto
                {
                    User = x,
                    Following = _currentUser.User.FollowingIds.Contains(x.Id)
                }).ToList()
            };
        }

        [HttpGet]
        [Route("getFollowings/{userId}")]
        public async Task<AllUsersDto> GetFollowings(string userId)
        {
            var result = await _userService.GetFollowings(userId);
            
            return new AllUsersDto
            {
                AllUsers = result.Select(x => new UserDto
                {
                    User = x,
                    Following = _currentUser.User.FollowingIds.Contains(x.Id)
                }).ToList()
            };
        }
    }

    public class EntriesCountDto
    {
        public long Count { get; set; }
    }

    public class NotificationsList
    {
        public List<UserNotification> Notifications { get; set; }
    }

    public class UserWithToken
    {
        public string Token { get; set; }

        public User User { get; set; }
    }

    public class AuthRequest
    {
        public string Name { get; set; }
        
        public string Password { get; set; }

        public string FirebaseToken { get; set; }
    }

    public class SignupRequest
    {
        public string Nickname { get; set; }

        public string Password { get; set; }

        public string FirebaseToken { get; set; }

        public string Email { get; set; }
    }

    public class UpdateProfileRequest
    {
        public IFormFile Image { get; set; }

        public string DisplayName { get; set; }

        public string Nickname { get; set; }

        public string About { get; set; }
    }
}