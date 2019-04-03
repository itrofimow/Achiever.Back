using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;

namespace Achiever.Core
{
    public interface IAchievementService
    {
        Task<List<Achievement>> GetAll();

        Task<List<Achievement>> GetAllByCategory(string categoryId);

        Task<Achievement> GetById(string id);

        Task Create(Achievement achievement);
    }
    
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;

        public AchievementService(IAchievementRepository achievementRepository)
        {
            _achievementRepository = achievementRepository;
        }

        public Task<List<Achievement>> GetAll()
        {
            return _achievementRepository.GetAll();
        }

        public Task<List<Achievement>> GetAllByCategory(string categoryId)
        {
            return _achievementRepository.GetAllByCategory(categoryId);
        }

        public Task<Achievement> GetById(string id)
        {
            return _achievementRepository.Get(id);
        }

        public Task Create(Achievement achievement)
        {
            return _achievementRepository.Create(achievement);
        }
    }
}