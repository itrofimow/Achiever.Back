using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;

namespace Achiever.Core
{
    public interface IAchievementCategoryService
    {
        Task Create(AchievementCategory category);

        Task<AchievementCategory> GetById(string id);

        Task<List<AchievementCategory>> GetAll();
    }
    
    public class AchievementCategoryService : IAchievementCategoryService
    {
        private readonly IAchievementCategoryRepository _achievementCategoryRepository;

        public AchievementCategoryService(IAchievementCategoryRepository achievementCategoryRepository)
        {
            _achievementCategoryRepository = achievementCategoryRepository;
        }
        
        public Task Create(AchievementCategory category)
        {
            return _achievementCategoryRepository.Create(category);
        }

        public Task<AchievementCategory> GetById(string id)
        {
            return _achievementCategoryRepository.GetById(id);
        }

        public Task<List<AchievementCategory>> GetAll()
        {
            return _achievementCategoryRepository.GetAll();
        }
    }
}