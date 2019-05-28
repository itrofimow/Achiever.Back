using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Feed;
using Achiever.Core.Models;
using Achiever.Core.Models.Feed;
using Achiever.Utils;
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
        private readonly IImageProcessingService _imageProcessingService;

        private const int PageSize = 20;

        public FeedController(
            IAcquiredAchievementRepository acquiredAchievementRepository,
            IFeedService feedService,
            IFileService fileService,
            ICurrentUser currentUser,
            IImageProcessingService imageProcessingService)
        {
            _acquiredAchievementRepository = acquiredAchievementRepository;
            _feedService = feedService;
            _fileService = fileService;
            _currentUser = currentUser;
            _imageProcessingService = imageProcessingService;
        }
        
        [HttpGet]
        [Route("one/{index}")]
        public async Task<FeedPageResponse> GetMyOne(int index, string startedAt)
        {
            var startedAtTime = string.IsNullOrEmpty(startedAt) 
                ? DateTime.Now 
                : DateTimeUtils.ParseIso8601(startedAt);
            
            var entries = await _feedService.GetFeedPageByOwner(_currentUser.User.Id, startedAtTime,
                PageSize * index, PageSize);
            MapFeedEntries(entries);

            return new FeedPageResponse
            {
                Entries = entries,
                StartedAt = startedAtTime
            };
        }

        [HttpGet]
        [Route("authorOne/{index}/{authorId}")]
        public async Task<FeedPageResponse> GetAuthorOne(int index, string authorId, string startedAt)
        {
            var startedAtTime = string.IsNullOrEmpty(startedAt) 
                ? DateTime.Now 
                : DateTimeUtils.ParseIso8601(startedAt);
            
            var entries = await _feedService.GetFeedPageByAuthor(authorId, startedAtTime, 
                PageSize * index, PageSize);
            MapFeedEntries(entries);
            
            return new FeedPageResponse
            {
                Entries = entries,
                StartedAt = startedAtTime
            };
        }

        [HttpGet]
        [Route("achievementOne/{index}/{achievementId}")]
        public async Task<FeedPageResponse> GetAchievementOne(int index, string achievementId, string startedAt)
        {
            var startedAtTime = string.IsNullOrEmpty(startedAt) 
                ? DateTime.Now 
                : DateTimeUtils.ParseIso8601(startedAt);
            
            var entries = await _feedService.GetFeedPageByAchievement(achievementId, startedAtTime,
                PageSize * index, PageSize);
            MapFeedEntries(entries);
            
            return new FeedPageResponse
            {
                Entries = entries,
                StartedAt = startedAtTime
            };
        }

        private void MapFeedEntries(List<FeedEntryResponse> entries)
        {
            entries.ForEach(x => x.IsLiked = x.Entry.Likes.Contains(_currentUser.UserId));
            entries.ForEach(x => x.Entry.LikedUsers.ForEach(
                y => y.Following = _currentUser.User.FollowingIds.Contains(y.User.Id)));
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

        [RequestSizeLimit(32 * 1000 * 1000)]
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
                var imagesCopyTasks = request.Files.Select(async file =>
                {
                    var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    
                    return await _imageProcessingService.SaveImage(ms);
                });
                feedEntry.Images = (await Task.WhenAll(imagesCopyTasks))
                    .Select(x => x.ImagePath).ToList();
            }

            await _feedService.CreateEntry(_currentUser.User, feedEntry);
        }

        [HttpGet]
        [Route("likes/{feedEntryId}")]
        public async Task<AllUsersDto> GetLikes(string feedEntryId)
        {
            var likes = await _feedService.GetLikes(feedEntryId);

            return new AllUsersDto
            {
                AllUsers = likes.Select(x => new UserDto
                    {
                        Following = _currentUser.User.FollowingIds.Contains(x.Id),
                        User = x
                    })
                    .ToList()
            };
        }
    }

    public class FeedPageResponse
    {
        public DateTime StartedAt { get; set; }
        public List<FeedEntryResponse> Entries { get; set; }
    }

    public class CreateFeedEntryByAchievementRequest
    {
        public string AchievementId { get; set; }

        public List<IFormFile> Files { get; set; }

        public String Comment { get; set; }
    }
}
