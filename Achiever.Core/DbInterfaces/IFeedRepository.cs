using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.User;

namespace Achiever.Core.DbInterfaces
{
    public interface IFeedRepository
    {
        Task CreateEntry(FeedEntry entry);

        Task<List<FeedEntry>> GetAllByAuthor(string authorId);

        Task<List<FeedEntry>> GetByIds(List<string> ids);

        Task<List<string>> GetForAuthor(string authorId, DateTime startTime, int skip, int limit);

        Task<bool> LikeOrUnlike(string userId, string feedEntryId);

        Task AddComment(string feedEntryId, FeedEntryComment comment);

        Task<long> CountByAuthor(string authorId);
    }
}