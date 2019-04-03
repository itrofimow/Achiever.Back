using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class AchievementCategoryRepository : IAchievementCategoryRepository
    {
        private readonly MongoContext _context;

        public AchievementCategoryRepository(MongoContext context)
        {
            _context = context;
        }
        
        public Task Create(AchievementCategory category)
        {
            return _context.For<AchievementCategory>()
                .InsertOneAsync(category);
        }

        public Task<AchievementCategory> GetById(string id)
        {
            return _context.For<AchievementCategory>()
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public Task<List<AchievementCategory>> GetAll()
        {
            return _context.For<AchievementCategory>()
                .Find(_ => true)
                .ToListAsync();
        }
    }
}