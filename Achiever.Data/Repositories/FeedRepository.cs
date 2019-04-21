using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using Achiever.Core.Models.Feed;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class FeedRepository : IFeedRepository
    {
        private readonly MongoContext _context;

        public FeedRepository(
            MongoContext context)
        {
            _context = context;
            CreateIndexes();
        }

        public Task CreateEntry(FeedEntry entry)
        {   
            return _context.For<FeedEntry>()
                .InsertOneAsync(entry);
        }

        public Task<List<FeedEntry>> GetAllByAuthor(string authorId)
        {
            return _context.For<FeedEntry>()
                .Find(x => x.AuthorId == authorId)
                .ToListAsync();
        }

        public Task<List<FeedEntry>> GetByIds(List<string> ids)
        {
            var filter = Builders<FeedEntry>.Filter.Where(x => ids.Contains(x.Id));
            
            return _context.For<FeedEntry>().Aggregate()
                .Match(filter)
                .Lookup(
                    foreignCollection: _context.For<Achievement>(),
                    localField: l => l.AchievementId,
                    foreignField: f => f.Id,
                    @as: (LookupFeedEntry x) => x.Achievements)
                .Project(p => new FeedEntry
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    AchievementId = p.AchievementId,
                    Achievement = p.Achievements.First(),
                    CreatedAt = p.CreatedAt,
                    Comment = p.Comment,
                    Images = p.Images,
                    
                    Likes = p.Likes,
                    LikesCount = p.Likes.Count,
                    
                    Comments = p.Comments,
                    CommentsCount = p.Comments.Count
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetForAuthor(string authorId, DateTime startTime,
            int skip, int limit)
        {
            var filter = Builders<FeedEntry>.Filter.Where(
                x => x.AuthorId == authorId && x.CreatedAt <= startTime);

            var sort = Builders<FeedEntry>.Sort.Descending(x => x.CreatedAt);

            var cursor = await _context.For<FeedEntry>()
                .FindAsync(filter, new FindOptions<FeedEntry>
                {
                    Sort = sort,
                    Skip = skip,
                    Limit = limit,
                    Projection = Builders<FeedEntry>.Projection.Include(x => x.Id)
                });

            var entries = await cursor.ToListAsync(); 

            return entries.Select(x => x.Id).ToList();
        }

        public async Task<bool> LikeOrUnlike(string userId, string feedEntryId)
        {
            var update = await _context.For<FeedEntry>().UpdateOneAsync(
                x => x.Id == feedEntryId,
                Builders<FeedEntry>.Update.Pull(x => x.Likes, userId));

            if (update.ModifiedCount == 0)
            {
                await _context.For<FeedEntry>().UpdateOneAsync(
                    x => x.Id == feedEntryId,
                    Builders<FeedEntry>.Update.AddToSet(x => x.Likes, userId));
                return true;
            }

            return false;
        }

        public Task AddComment(string feedEntryId, FeedEntryComment comment)
        {
            var filter = Builders<FeedEntry>.Filter.Where(x => x.Id == feedEntryId);

            var update = Builders<FeedEntry>.Update.Push(x => x.Comments, comment);

            return _context.For<FeedEntry>()
                .UpdateOneAsync(filter, update);
        }

        public async Task<List<(string, string)>> GetForAchievement(string achievementId, DateTime startTime,
            int skip, int limit)
        {
            var filter = Builders<FeedEntry>.Filter.Where(
                x => x.AchievementId == achievementId && x.CreatedAt <= startTime);

            var sort = Builders<FeedEntry>.Sort.Descending(x => x.CreatedAt);

            var cursor = await _context.For<FeedEntry>()
                .FindAsync(filter, new FindOptions<FeedEntry>
                {
                    Sort = sort,
                    Skip = skip,
                    Limit = limit,
                    Projection = 
                        Builders<FeedEntry>.Projection.Combine(
                            Builders<FeedEntry>.Projection.Include(x => x.Id),
                            Builders<FeedEntry>.Projection.Include(x => x.AuthorId))
                });

            var entries = await cursor.ToListAsync(); 

            return entries.Select(x => 
                (achievementId: x.Id, authorId: x.AuthorId))
                .ToList();
        }

        public Task<long> CountByAuthor(string authorId)
        {
            return _context.For<FeedEntry>()
                .CountDocumentsAsync(x => x.AuthorId == authorId);
        }

        private void CreateIndexes()
        {
            _context.For<FeedEntry>().Indexes.CreateMany(
                new []
                {
                    new CreateIndexModel<FeedEntry>(
                        Builders<FeedEntry>.IndexKeys.Ascending(x => x.AuthorId)), 
                });

            _context.For<GlobalFeedEntry>().Indexes.CreateMany(
                new[]
                {
                    new CreateIndexModel<GlobalFeedEntry>(
                        Builders<GlobalFeedEntry>.IndexKeys.Ascending(x => x.FeedOwnerId)), 
                }
            );
        }
    }

    public class LookupFeedEntry
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AchievementId { get; set; }

        public List<Achievement> Achievements { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Comment { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public List<string> Likes { get; set; } = new List<string>();

        public List<FeedEntryComment> Comments { get; set; } = new List<FeedEntryComment>();
    }
}
