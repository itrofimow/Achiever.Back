using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using Achiever.Core.Models.User;

namespace Achiever.Core
{
    public interface ISearchService
    {
        Task<List<User>> SearchUsers(string query);

        Task<List<Achievement>> SearchAchievements(string query);
    }
    
    public class SearchService : ISearchService
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IUserRepository _userRepository;

        public SearchService(
            IAchievementRepository achievementRepository,
            IUserRepository userRepository)
        {
            _achievementRepository = achievementRepository;
            _userRepository = userRepository;
        }

        public async Task<List<User>> SearchUsers(string query)
        {
            var allUsers = await _userRepository.GetAll();

            return allUsers.Where(x => UserMatchesQuery(x, query)).ToList();
        }

        public async Task<List<Achievement>> SearchAchievements(string query)
        {
            var allAchievements = await _achievementRepository.GetAll();

            return allAchievements.Where(x => AchievementMatchesQuery(x, query)).ToList();
        }

        private bool UserMatchesQuery(User user, string query)
        {
            return user.Nickname.Contains(query, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool AchievementMatchesQuery(Achievement achievement, string query)
        {
            return achievement.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase) |
                   achievement.Description.Contains(query, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}