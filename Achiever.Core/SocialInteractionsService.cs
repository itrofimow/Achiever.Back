using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Notifications;

namespace Achiever.Core
{
    public interface ISocialInteractionsService
    {
        Task Follow(string userId, string targetId);

        Task Unfollow(string userId, string targetId);
    }
    
    public class SocialInteractionsService : ISocialInteractionsService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFeedRepository _feedRepository;
        private readonly IGlobalFeedRepository _globalFeedRepository;
        private readonly INotificationService _notificationService;

        public SocialInteractionsService(
            IUserRepository userRepository,
            IFeedRepository feedRepository,
            IGlobalFeedRepository globalFeedRepository,
            INotificationService notificationService
        )
        {
            _userRepository = userRepository;
            _feedRepository = feedRepository;
            _globalFeedRepository = globalFeedRepository;
            _notificationService = notificationService;
        }
            
        public async Task Follow(string userId, string targetId)
        {
            if (userId == targetId) return;
            
            await _userRepository.Follow(userId, targetId);
            var user = await _userRepository.GetById(userId);
            await _notificationService.ProcessFollow(user, targetId);

            var targetPosts = await _feedRepository.GetAllByAuthor(targetId);
            await _globalFeedRepository.CreateEntriesForFollower(userId, targetPosts);
        }

        public async Task Unfollow(string userId, string targetId)
        {
            if (userId == targetId) return;
            
            await _userRepository.Unfollow(userId, targetId);

            await _globalFeedRepository.DeleteAllEntriesForFollower(targetId, userId);
        }
    }
}