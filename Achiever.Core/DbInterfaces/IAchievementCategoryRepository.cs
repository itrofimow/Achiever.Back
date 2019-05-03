using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models;

namespace Achiever.Core.DbInterfaces
{
    public interface IAchievementCategoryRepository
    {
        Task Create(AchievementCategory category);

        Task<AchievementCategory> GetById(string id);

        Task<List<AchievementCategory>> GetByIds(List<string> ids);

        Task<List<AchievementCategory>> GetAll();
    }
}