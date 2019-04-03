using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using MongoDB.Driver;

namespace Achiever.Data.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly MongoContext _context;

        public AchievementRepository(MongoContext context)
        {
            _context = context;
            
            CreateIndexes();
        }
        
        public Task Create(Achievement achievement)
        {
            return _context.For<Achievement>()
                .InsertOneAsync(achievement);
        }

        public Task<Achievement> Get(string id)
        {
            var aggregation = _context.For<Achievement>().Aggregate()
                .Match(x => x.Id == id);

            return LookupProject(aggregation)
                .SingleOrDefaultAsync();
        }

        public Task<List<Achievement>> GetAll()
        {
            var aggregation = _context.For<Achievement>().Aggregate()
                .Match(_ => true);

            return LookupProject(aggregation)
                .ToListAsync();
        }

        public Task<List<Achievement>> GetAllByCategory(string categoryId)
        {
            return _context.For<Achievement>()
                .Find(x => x.AchievementCategoryId == categoryId)
                .ToListAsync();
        }
        
        private void CreateIndexes()
        {
            _context.For<Achievement>().Indexes.CreateMany(
                new []
                {
                    new CreateIndexModel<Achievement>(Builders<Achievement>.IndexKeys
                        .Ascending(x => x.AchievementCategoryId))
                });
        }
        
        private IAggregateFluent<Achievement> LookupProject(IAggregateFluent<Achievement> aggregation)
        {
            return aggregation
                .Lookup(
                    foreignCollection: _context.For<AchievementCategory>(),
                    localField: l => l.AchievementCategoryId,
                    foreignField: f => f.Id,
                    @as: (LookupAchievement x) => x.Categories
                )
                .Project(p => new Achievement
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    HasDefaultBackground = p.HasDefaultBackground,
                    DefaultBackgroundColor = p.DefaultBackgroundColor,
                    BackgroundImage = p.BackgroundImage,
                    FrontImage = p.FrontImage,
                    BigImage = p.BigImage,
                    AchievementCategoryId = p.AchievementCategoryId,
                    Category = p.Categories.First()
                });
        }
    }

    internal class LookupAchievement : Achievement
    {
        public List<AchievementCategory> Categories { get; set; }
    }
}