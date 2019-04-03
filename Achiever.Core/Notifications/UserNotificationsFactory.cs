using System;
using Achiever.Core.Models;

namespace Achiever.Core.Notifications
{
    public interface IUserNotificationsFactory
    {
        UserNotification CreateLikeNotification(string userId, string targetUserId, string feedEntryId);

        UserNotification CreateCommentNotification(string userId, string targetUserId, 
            string feedEntryId, string commentText);

        UserNotification CreateFollowNotification(string userId, string targetUserId);
    }
    
    public class UserNotificationsFactory : IUserNotificationsFactory
    {   
        public UserNotification CreateLikeNotification(string userId, string targetUserId, string feedEntryId)
        {
            var notification = new UserNotification
            {
                UserId = targetUserId,
                AuthorId = userId,
                FeedEntryId = feedEntryId,
                Text = "понравилась ваша запись",
                
                CreatedAt = DateTime.Now,
                NotificationType = UserNotificationType.Like
            };

            return notification;
        }

        public UserNotification CreateCommentNotification(string userId, string targetUserId, string feedEntryId, 
            string commentText)
        {
            var notification = new UserNotification
            {
                UserId = targetUserId,
                AuthorId = userId,
                FeedEntryId = feedEntryId,
                Text = commentText,
                
                CreatedAt = DateTime.Now,
                NotificationType = UserNotificationType.Comment
            };

            return notification;
        }

        public UserNotification CreateFollowNotification(string userId, string targetUserId)
        {
            var notification = new UserNotification
            {
                AuthorId = userId,
                UserId = targetUserId,
                Text = "теперь ваш подписчик",
                
                CreatedAt = DateTime.Now,
                NotificationType = UserNotificationType.Comment
            };

            return notification;
        }
    }
}