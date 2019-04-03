using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using Achiever.Core.Models.Feed;
using Achiever.Core.Models.User;

namespace Achiever.Core.Notifications
{
    public interface INotificationService
    {
        Task ProcessLike(User user, string targetUserId, string feedEntryId);

        Task ProcessComment(User user, string targetUserId, string feedEntryId, FeedEntryComment comment);

        Task ProcessFollow(User user, string targetUserId);
    }
    
    public class NotificationService : INotificationService
    {
        private readonly INotificationSender _notificationSender;
        private readonly IUserNotificationsRepository _userNotificationsRepository;
        private readonly IUserNotificationsFactory _userNotificationsFactory;

        public NotificationService(
            INotificationSender notificationSender,
            IUserNotificationsRepository userNotificationsRepository,
            IUserNotificationsFactory userNotificationsFactory)
        {
            _notificationSender = notificationSender;
            _userNotificationsRepository = userNotificationsRepository;
            _userNotificationsFactory = userNotificationsFactory;
        }
        
        public async Task ProcessLike(User user, string targetUserId, string feedEntryId)
        {
            await _notificationSender.SendLike(user.Nickname, targetUserId, feedEntryId);

            var notification = _userNotificationsFactory.CreateLikeNotification(user.Id, targetUserId, feedEntryId);
            await _userNotificationsRepository.AddNotification(notification);
        }

        public async Task ProcessComment(User user, string targetUserId, string feedEntryId, FeedEntryComment comment)
        {
            await _notificationSender.SendComment(user.Nickname, targetUserId, feedEntryId);

            var notification = _userNotificationsFactory.CreateCommentNotification(user.Id, targetUserId, feedEntryId,
                comment.Text);
            await _userNotificationsRepository.AddNotification(notification);
        }

        public async Task ProcessFollow(User user, string targetUserId)
        {
            await _notificationSender.SendFollow(user.Nickname, targetUserId);

            var notification = _userNotificationsFactory.CreateFollowNotification(user.Id, targetUserId);
            await _userNotificationsRepository.AddNotification(notification);
        }
    }
}