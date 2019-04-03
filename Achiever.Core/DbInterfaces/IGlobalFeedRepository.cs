using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.User;

namespace Achiever.Core.DbInterfaces
{
    public interface IGlobalFeedRepository
    {
        Task CreateEntryForAllFollowers(User user, FeedEntry entry);

        Task DeleteEntry(FeedEntry entry);

        Task DeleteAllEntriesForFollower(string userId, string followerId);

        Task CreateEntriesForFollower(string followerId, List<FeedEntry> entries);

        Task<List<GlobalFeedEntry>> GetForOwner(string feedOwnerId, DateTime startTime, int skip, int limit);
    }
}