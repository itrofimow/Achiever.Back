using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Feed;
using Achiever.Core.Models;
using Achiever.Core.Models.Feed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Achiever.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase
    {
        private readonly IAcquiredAchievementRepository _acquiredAchievementRepository;
        private readonly IFeedService _feedService;
        private readonly IFileService _fileService;
        private readonly ICurrentUser _currentUser;

        private const int PageSize = 20;

        public FeedController(
            IAcquiredAchievementRepository acquiredAchievementRepository,
            IFeedService feedService,
            IFileService fileService,
            ICurrentUser currentUser)
        {
            _acquiredAchievementRepository = acquiredAchievementRepository;
            _feedService = feedService;
            _fileService = fileService;
            _currentUser = currentUser;
        }
        
        [HttpGet]
        [Route("one/{index}")]
        public async Task<FeedPageResponse> GetMyOne(int index)
        {
            var entries = await _feedService.GetFeedPageByOwner(_currentUser.User.Id, DateTime.Now,
                PageSize * index, PageSize);
            entries.ForEach(x => x.IsLiked = x.Entry.Likes.Contains(_currentUser.User.Id));

            return new FeedPageResponse
            {
                Entries = entries
            };
        }

        //[Authorize]
        [HttpGet]
        [Route("authorOne/{index}/{authorId}")]
        public async Task<FeedPageResponse> GetAuthorOne(int index, string authorId)
        {
            var entries = await _feedService.GetFeedPageByAuthor(authorId, DateTime.Now, 
                PageSize * index, PageSize);
            entries.ForEach(x => x.IsLiked = x.Entry.Likes.Contains(_currentUser.User.Id));
            
            return new FeedPageResponse
            {
                Entries = entries
            };
        }

        [HttpGet]
        [Route("achievementOne/{index}/{achievementId}")]
        public async Task<FeedPageResponse> GetAchievementOne(int index, string achievementId)
        {
            var entries = await _feedService.GetFeedPageByAchievement(achievementId, DateTime.Now,
                PageSize * index, PageSize);
            entries.ForEach(x => x.IsLiked = x.Entry.Likes.Contains(_currentUser.UserId));
            
            return new FeedPageResponse
            {
                Entries = entries
            };
        }

        [HttpPut]
        [Route("like/{feedEntryId}")]
        public async Task LikeOrUnlike(string feedEntryId)
        {
            await _feedService.LikeOrUnlike(_currentUser.User.Id, _currentUser.User.Nickname, feedEntryId);
        }

        [HttpPut]
        [Route("comment/{feedEntryId}")]
        public async Task<FeedEntryComment> AddComment(string feedEntryId, [FromBody] FeedEntryComment comment)
        {
            comment.AuthorId = _currentUser.UserId;
            comment.CreatedAt = DateTime.Now;
            await _feedService.AddComment(_currentUser.User.Nickname, feedEntryId, comment);

            comment.AuthorNickname = _currentUser.User.Nickname;
            comment.AuthorProfileImage = _currentUser.User.ProfileImagePath;

            return comment;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task CreateEntry([FromForm] CreateFeedEntryByAchievementRequest request)
        {
            if (request.Files?.Count > 10) throw new InvalidOperationException();
            
            var acquiredAchievement = new AcquiredAchievement
            {
                AchievementId = request.AchievementId,
                AcquiredAt = DateTime.Now,
                OwnerId = _currentUser.User.Id
            };
            await _acquiredAchievementRepository.Create(acquiredAchievement);

            var feedEntry = new FeedEntry
            {
                AchievementId = request.AchievementId,
                AuthorId = _currentUser.User.Id,
                CreatedAt = DateTime.Now,
                Comment = request.Comment,
                Images = new List<string>()
            };

            if (request.Files != null)
            {
                var imagesCopyTasks = request.Files.Select(file => _fileService.SaveFile(file.OpenReadStream()));
                feedEntry.Images = (await Task.WhenAll(imagesCopyTasks)).ToList();
            }

            await _feedService.CreateEntry(_currentUser.User, feedEntry);
        }
    }

    public class FeedPageResponse
    {
        public List<FeedEntryResponse> Entries { get; set; }
    }

    public class CreateFeedEntryByAchievementRequest
    {
        public string AchievementId { get; set; }

        public List<IFormFile> Files { get; set; }

        public String Comment { get; set; }
    }
}
