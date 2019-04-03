using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using Achiever.Core.Models.User;

namespace Achiever.Core
{
    public interface IAcquiredAchievementService
    {
        Task<List<Achievement>> GetByOwnerAndCategory(string ownerId, string categoryId);

        Task<List<User>> GetFollowingsWhoHave(string achievementId, string userId);
    }
    
    public class AcquiredAchievementService : IAcquiredAchievementService
    {
        private readonly IAcquiredAchievementRepository _acquiredAchievementRepository;
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository;

        public AcquiredAchievementService(
            IAcquiredAchievementRepository acquiredAchievementRepository,
            IAchievementRepository achievementRepository,
            IUserRepository userRepository)
        {
            _acquiredAchievementRepository = acquiredAchievementRepository;
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
        }
        
        public async Task<List<Achievement>> GetByOwnerAndCategory(string ownerId, string categoryId)
        {
            var allAchievements = (await _acquiredAchievementRepository.GetByOwner(ownerId))
                .GroupBy(x => x.AchievementId)
                .ToDictionary(g => g.Key, g => g.First());

            var allByCategory = await _achievementRepository.GetAllByCategory(categoryId);

            return allByCategory.Where(x => allAchievements.ContainsKey(x.Id)).ToList();
        }

        public async Task<List<User>> GetFollowingsWhoHave(string achievementId, string userId)
        {
            var user = await _userRepository.GetById(userId);

            var acquiredByFollowings = await _acquiredAchievementRepository.GetByOwnersAndAchievementId(achievementId,
                user.FollowingIds.ToList());
            var ownersIds = acquiredByFollowings.Select(x => x.OwnerId)
                .Distinct().ToList();

            return await _userRepository.GetByIds(ownersIds);
        }
    }
}