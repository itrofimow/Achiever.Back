using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models;
using Achiever.Core.Models.User;

namespace Achiever.Core.DbInterfaces
{
    public interface IUserRepository
    {
        Task<User> GetById(string id);

        Task<string> GetFirebaseToken(string userid);

        Task SetFirebaseToken(string userId, string firebaseToken);

        Task<User> GetByNickname(string nickname);

        Task<User> GetByCredentials(string nickname, string password);
        
        Task Create(string nickname, string password);

        Task UpdateProfileImage(string userId, string profileImagePath);

        Task<List<User>> GetByIds(List<string> ids);

        Task Follow(string userId, string targetId);

        Task Unfollow(string userId, string targetId);

        Task<User> Update(string userId, User model);

        Task<List<User>> GetAll();
    }
}