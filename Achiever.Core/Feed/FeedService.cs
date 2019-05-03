using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.User;
using Achiever.Core.Notifications;

namespace Achiever.Core.Feed
{
    public interface IFeedService
    {
        Task CreateEntry(User user, FeedEntry entry);

        Task<List<FeedEntryResponse>> GetFeedPageByOwner(string feedOwnerId, DateTime startTime, int skip, int limit);

        Task<List<FeedEntryResponse>> GetFeedPageByAuthor(string authorId, DateTime startTime, int skip, int limit);

        Task<List<FeedEntryResponse>> GetFeedPageByAchievement(string achievementId, DateTime startTime,
            int skip, int limit);

        Task LikeOrUnlike(string userId, string userNickname, string feedEntryId);

        Task AddComment(string userNickname, string feedEntryId, FeedEntryComment comment);

        Task<List<User>> GetLikes(string feedEntryId);
    }
    
    public class FeedService : IFeedService
    {
        private readonly IFeedRepository _feedRepository;
        private readonly IGlobalFeedRepository _globalFeedRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAchievementCategoryRepository _achievementCategoryRepository;

        public FeedService(
            IFeedRepository feedRepository,
            IGlobalFeedRepository globalFeedRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IAchievementCategoryRepository achievementCategoryRepository)
        {
            _feedRepository = feedRepository;
            _globalFeedRepository = globalFeedRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _achievementCategoryRepository = achievementCategoryRepository;
        }

        public async Task CreateEntry(User user, FeedEntry entry)
        {
            entry.AuthorId = user.Id;

            await _feedRepository.CreateEntry(entry);
            await _globalFeedRepository.CreateEntryForAllFollowers(user, entry);
        }

        public async Task LikeOrUnlike(string userId, string userNickname, string feedEntryId)
        {
            var liked = await _feedRepository.LikeOrUnlike(userId, feedEntryId);
            if (!liked) return;

            var feedEntry = (await _feedRepository.GetByIds(new List<string> {feedEntryId})).First();
            var user = await _userRepository.GetByNickname(userNickname);

            if (userId != feedEntry.AuthorId)
                await _notificationService.ProcessLike(user, feedEntry.AuthorId, feedEntryId);
        }

        public async Task AddComment(string userNickname, string feedEntryId, FeedEntryComment comment)
        {
            await _feedRepository.AddComment(feedEntryId, comment);

            var feedEntry = (await _feedRepository.GetByIds(new List<string> {feedEntryId})).First();
            var user = await _userRepository.GetByNickname(userNickname);

            if (comment.AuthorId != feedEntry.AuthorId)
                await _notificationService.ProcessComment(user, feedEntry.AuthorId, feedEntryId, comment);
        }

        public async Task<List<User>> GetLikes(string feedEntryId)
        {
            var feedEntry = (await _feedRepository.GetByIds(new List<string> {feedEntryId}))
                .Single();

            return await _userRepository.GetByIds(feedEntry.Likes);
        }

        public async Task<List<FeedEntryResponse>> GetFeedPageByOwner(string feedOwnerId, DateTime startTime, int skip, int limit)
        {
            var globalEntries = await _globalFeedRepository.GetForOwner(feedOwnerId, startTime, skip, limit);

            var entriesTask = GetByIds(globalEntries.Select(x => x.FeedEntryId).ToList());
            var authorsTask = _userRepository.GetByIds(globalEntries.Select(x => x.AuthorId).ToList());

            await Task.WhenAll(entriesTask, authorsTask);
            
            var entries = entriesTask.Result;
            var authors = authorsTask.Result;
            
            return entries.OrderByDescending(x => x.CreatedAt)
                .Select(y =>
                {
                    var author = authors.Single(x => x.Id == y.AuthorId);
                    return new FeedEntryResponse
                    {
                        AuthorNickname = author.Nickname,
                        AuthorProfileImagePath = author.ProfileImagePath,
                        Entry = y,
                    };
                })
                .ToList();
        }

        public async Task<List<FeedEntryResponse>> GetFeedPageByAuthor(string authorId, DateTime startTime, int skip,
            int limit)
        {
            var idsTask = _feedRepository.GetForAuthor(authorId, startTime, skip, limit);
            var authorTask = _userRepository.GetByIds(new List<string>{authorId});

            await Task.WhenAll(idsTask, authorTask);

            var ids = idsTask.Result;
            var author = authorTask.Result.First();

            var entries = await GetByIds(ids);

            return entries.OrderByDescending(x => x.CreatedAt)
                .Select(y => new FeedEntryResponse
                {
                    AuthorNickname = author.Nickname,
                    AuthorProfileImagePath = author.ProfileImagePath,
                    Entry = y,
                })
                .ToList();
        }

        public async Task<List<FeedEntryResponse>> GetFeedPageByAchievement(string achievementId, DateTime startTime,
            int skip, int limit)
        {
            var ids = await _feedRepository.GetForAchievement(achievementId, startTime, skip, limit);

            var entriesTask = GetByIds(ids.Select(x => x.Item1).ToList());
            var authorsTask = _userRepository.GetByIds(ids.Select(x => x.Item2).ToList());

            await Task.WhenAll(entriesTask, authorsTask);
            
            var entries = entriesTask.Result;
            var authors = authorsTask.Result;
            
            return entries.OrderByDescending(x => x.CreatedAt)
                .Select(y =>
                {
                    var author = authors.Single(x => x.Id == y.AuthorId);
                    return new FeedEntryResponse
                    {
                        AuthorNickname = author.Nickname,
                        AuthorProfileImagePath = author.ProfileImagePath,
                        Entry = y,
                    };
                })
                .ToList();
        }

        private async Task<List<FeedEntry>> GetByIds(List<string> ids)
        {
            var entries = await _feedRepository.GetByIds(ids);

            await Task.WhenAll(
                ProcessAuthors(entries),
                ProcessLikes(entries),
                ProcessCategories(entries));

            return entries;
        }

        private async Task ProcessAuthors(List<FeedEntry> entries)
        {
            var allCommentAuthorsIds = entries.SelectMany(x => x.Comments).Select(y => y.AuthorId)
                .Distinct().ToList();
            var allCommentAuthors = await _userRepository.GetByIds(allCommentAuthorsIds);
            
            var allCommentAuthorsDict = new Dictionary<string, User>();
            allCommentAuthors.ForEach(x => allCommentAuthorsDict[x.Id] = x);
            
            entries.ForEach(x => x.Comments.ForEach(y =>
            {
                var author = allCommentAuthorsDict[y.AuthorId];

                y.AuthorNickname = author.Nickname;
                y.AuthorProfileImage = author.ProfileImagePath;
            }));
        }

        private async Task ProcessLikes(List<FeedEntry> entries)
        {
            var allLikedIds = entries.SelectMany(x => x.Likes)
                .Distinct().ToList();
            var allLikedUsers = await _userRepository.GetByIds(allLikedIds);
            
            var allLikedUsersDict = new Dictionary<string, User>();
            allLikedUsers.ForEach(x => allLikedUsersDict[x.Id] = x);
            
            entries.ForEach(x => { 
                x.LikedUsers = x.Likes.Select(y => new CoreUserDto
                {
                    User = allLikedUsersDict[y]
                }).ToList();   
            });
        }

        private async Task ProcessCategories(List<FeedEntry> entries)
        {
            var allCategoriesIds = entries.Select(x => x.Achievement.AchievementCategoryId)
                .Distinct().ToList();
            var allCategories = await _achievementCategoryRepository.GetByIds(allCategoriesIds);

            var allCategoriesDict = allCategories.ToDictionary(x => x.Id);
            
            entries.ForEach(x => { x.Achievement.Category = allCategoriesDict[x.Achievement.AchievementCategoryId]; });
        }
    }
}