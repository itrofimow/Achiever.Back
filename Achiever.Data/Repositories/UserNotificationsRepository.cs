using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class UserNotificationsRepository : IUserNotificationsRepository
    {
        private readonly MongoContext _context;

        public UserNotificationsRepository(MongoContext context)
        {
            _context = context;
            
            CreateIndexes();
        }
        
        public Task AddNotification(UserNotification notification)
        {
            return _context.For<UserNotification>()
                .InsertOneAsync(notification);
        }

        public Task<List<UserNotification>> GetAllForUser(string userId)
        {
            return _context.For<UserNotification>()
                .Find(x => x.UserId == userId)
                .Sort(Builders<UserNotification>.Sort.Descending(x => x.CreatedAt))
                .ToListAsync();
        }

        private void CreateIndexes()
        {
            _context.For<UserNotification>().Indexes.CreateMany(
                new []
                {
                    new CreateIndexModel<UserNotification>(
                        Builders<UserNotification>.IndexKeys
                            .Ascending(x => x.AuthorId)
                            .Descending(x => x.CreatedAt)), 
                });
        }
    }
}