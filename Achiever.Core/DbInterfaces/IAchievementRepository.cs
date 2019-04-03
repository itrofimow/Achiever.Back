using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models;

namespace Achiever.Core.DbInterfaces
{
    public interface IAchievementRepository
    {
        Task Create(Achievement achievement);

        Task<Achievement> Get(string id);

        Task<List<Achievement>> GetAll();

        Task<List<Achievement>> GetAllByCategory(string categoryId);
    }
}