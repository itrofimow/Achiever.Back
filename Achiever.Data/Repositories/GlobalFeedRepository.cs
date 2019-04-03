using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.User;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class GlobalFeedRepository : IGlobalFeedRepository
    {
        private readonly MongoContext _context;

        public GlobalFeedRepository(MongoContext context)
        {
            _context = context;
            CreateIndexes();
        }

        public Task CreateEntryForAllFollowers(User user, FeedEntry entry)
        {
            var followersIds = user.GetAllFollowers();
            var now = DateTime.Now;
            
            var entries = followersIds.Select(
                x => new GlobalFeedEntry
                {
                    CreatedAt = entry.CreatedAt,
                    FeedEntryId = entry.Id,
                    FeedOwnerId = x,
                    AuthorId = user.Id,
                    InsertedAt = now
                }).ToList();

            if (entries.Count > 0)
                return _context.For<GlobalFeedEntry>()
                    .InsertManyAsync(entries);

            return Task.CompletedTask;
        }

        public Task DeleteEntry(FeedEntry entry)
        {
            var filter = Builders<GlobalFeedEntry>.Filter
                .Where(x => x.FeedEntryId == entry.Id);
            
            return _context.For<GlobalFeedEntry>()
                .DeleteManyAsync(filter);
        }

        public Task DeleteAllEntriesForFollower(string userId, string followerId)
        {
            var filter = Builders<GlobalFeedEntry>.Filter
                .Where(x => x.FeedOwnerId == followerId && x.AuthorId == userId);

            return _context.For<GlobalFeedEntry>()
                .DeleteManyAsync(filter);
        }

        public Task CreateEntriesForFollower(string followerId, List<FeedEntry> entries)
        {
            var now = DateTime.Now;
            
            var globalEntries = entries.Select(x =>
                new GlobalFeedEntry
                {
                    AuthorId = x.AuthorId,
                    FeedEntryId = x.Id,
                    FeedOwnerId = followerId,
                    CreatedAt = x.CreatedAt,
                    InsertedAt = now
                }).ToList();

            if (globalEntries.Count > 0)
                return _context.For<GlobalFeedEntry>()
                    .InsertManyAsync(globalEntries);

            return Task.CompletedTask;
        }

        public async Task<List<GlobalFeedEntry>> GetForOwner(string feedOwnerId, DateTime startTime, 
            int skip, int limit)
        {
            var filter = Builders<GlobalFeedEntry>.Filter.Where(
                x => x.FeedOwnerId == feedOwnerId && x.InsertedAt <= startTime);

            var sort = Builders<GlobalFeedEntry>.Sort.Descending(x => x.CreatedAt);

            var cursor = await _context.For<GlobalFeedEntry>()
                .FindAsync(filter, new FindOptions<GlobalFeedEntry>
                {
                    Sort = sort,
                    Skip = skip,
                    Limit = limit
                });

            return await cursor.ToListAsync();

        }

        private void CreateIndexes()
        {
            _context.For<GlobalFeedEntry>().Indexes.CreateMany(
                new []
                {
                    new CreateIndexModel<GlobalFeedEntry>(Builders<GlobalFeedEntry>.IndexKeys
                        .Ascending(x => x.FeedOwnerId).Ascending(x => x.AuthorId)),
                    new CreateIndexModel<GlobalFeedEntry>(Builders<GlobalFeedEntry>.IndexKeys
                        .Ascending(x => x.FeedEntryId)),
                    new CreateIndexModel<GlobalFeedEntry>(Builders<GlobalFeedEntry>.IndexKeys
                        .Ascending(x => x.FeedOwnerId).Descending(x => x.CreatedAt)),
                    new CreateIndexModel<GlobalFeedEntry>(Builders<GlobalFeedEntry>.IndexKeys
                        .Ascending(x => x.FeedOwnerId).Ascending(x => x.InsertedAt)), 
                });
        }
    }
}