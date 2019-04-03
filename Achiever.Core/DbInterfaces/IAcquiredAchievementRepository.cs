using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models;

namespace Achiever.Core.DbInterfaces
{
    public interface IAcquiredAchievementRepository
    {
        Task<AcquiredAchievement> Get(string id);

        Task Create(AcquiredAchievement achievement);

        Task<List<AcquiredAchievement>> GetByOwner(string ownerId);

        Task<List<AcquiredAchievement>> GetByOwnersAndAchievementId(string achievementId, List<string> ownersIds);
    }
}