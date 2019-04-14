using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models;
using Achiever.Core.Models.User;

namespace Achiever.Core
{
    public interface IUserService
    {
        Task<User> Authenticate(string nickname, string password, string firebaseToken);
        
        Task<User> GetById(string id);

        Task<User> GetByNickname(string nickname);

        Task Create(string nickname, string password);

        Task UpdateProfileImage(string userId, string profileImagePath);

        Task<User> Update(string userId, User model);

        Task<List<UserNotification>> GetNotifications(string userId);

        Task<long> CountFeedEntries(string userId);

        Task<List<User>> GetFollowers(string userId);

        Task<List<User>> GetFollowings(string userId);
    }
    
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserNotificationsRepository _userNotificationsRepository;
        private readonly IFeedRepository _feedRepository;

        public UserService(
            IUserRepository userRepository,
            IUserNotificationsRepository userNotificationsRepository,
            IFeedRepository feedRepository)
        {
            _userRepository = userRepository;
            _userNotificationsRepository = userNotificationsRepository;
            _feedRepository = feedRepository;
        }

        public async Task<User> Authenticate(string nickname, string password, string firebaseToken)
        {
            var user = await _userRepository.GetByCredentials(nickname, password);
            if (user == null)
                throw new Exception("invalid creds");

            await _userRepository.SetFirebaseToken(user.Id, firebaseToken);

            return user;
        }

        public Task<User> GetById(string id)
        {
            return _userRepository.GetById(id);
        }

        public Task<User> GetByNickname(string nickname)
        {
            return _userRepository.GetByNickname(nickname);
        }

        public Task Create(string username, string password)
        {
            return _userRepository.Create(username, password);
        }

        public Task UpdateProfileImage(string userId, string profileImagePath)
        {
            return _userRepository.UpdateProfileImage(userId, profileImagePath);
        }

        public Task<User> Update(string userId, User model)
        {
            return _userRepository.Update(userId, model);
        }

        public async Task<List<UserNotification>> GetNotifications(string userId)
        {
            var notifications = await _userNotificationsRepository.GetAllForUser(userId);

            var authorsDict = (await _userRepository.GetByIds(notifications.Select(x => x.AuthorId).ToList()))
                .ToDictionary(x => x.Id);
            
            notifications.ForEach(x => x.Author = authorsDict[x.AuthorId]);

            return notifications;
        }

        public Task<long> CountFeedEntries(string userId)
        {
            return _feedRepository.CountByAuthor(userId);
        }

        public async Task<List<User>> GetFollowers(string userId)
        {
            var user = await _userRepository.GetById(userId);

            return await _userRepository.GetByIds(user.FollowersIds.ToList());
        }

        public async Task<List<User>> GetFollowings(string userId)
        {
            var user = await _userRepository.GetById(userId);

            return await _userRepository.GetByIds(user.FollowingIds.ToList());
        }
    }
}