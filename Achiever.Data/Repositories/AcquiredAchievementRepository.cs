using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class AcquiredAchievementRepository : IAcquiredAchievementRepository
    {
        private readonly MongoContext _context;
        
        public AcquiredAchievementRepository(MongoContext context)
        {
            _context = context;
        }

        public Task<AcquiredAchievement> Get(string id)
        {
            return _context.For<AcquiredAchievement>()
                .Find(x => x.Id == id).SingleOrDefaultAsync();
        }

        public Task Create(AcquiredAchievement achievement)
        {   
            return _context.For<AcquiredAchievement>()
                .InsertOneAsync(achievement);
        }

        public Task<List<AcquiredAchievement>> GetByOwner(string ownerId)
        {
            return _context.For<AcquiredAchievement>()
                .Find(x => x.OwnerId == ownerId)
                .ToListAsync();
        }

        public Task<List<AcquiredAchievement>> GetByOwnersAndAchievementId(string achievementId, List<string> ownersIds)
        {
            var filter =
                Builders<AcquiredAchievement>.Filter.Where(x => x.AchievementId == achievementId) &
                Builders<AcquiredAchievement>.Filter.In(x => x.OwnerId, ownersIds);

            return _context.For<AcquiredAchievement>()
                .Find(filter)
                .ToListAsync();
        }
    }
}