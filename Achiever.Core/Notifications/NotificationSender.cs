using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.DbInterfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;

namespace Achiever.Core.Notifications
{
    
    public class NotificationSender : INotificationSender
    {
        private static class NotificationTypes
        {
            public const string LikeNotification = "LikeNotification";

            public const string CommentNotification = "CommentNotification";

            public const string FollowNotification = "FollowNotification";
        }
        
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(
            IUserRepository userRepository,
            ILogger<NotificationSender> logger)
        {
            _userRepository = userRepository;
            _logger = logger;

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("Firebase/serviceAccountKey.json")
            });
        }

        public async Task SendLike(string userNickname, string targetUserId, string feedEntryId)
        {
            var userToken = await GetUserToken(targetUserId);
            
            var notification = new Notification
            {
                Title = userNickname,
                Body = "liked your post"
            };
            
            var data = new Dictionary<string, string>
            {
                {"type", NotificationTypes.LikeNotification},
                {"feedEntryId", feedEntryId}
            };

            await SendToUserInternal(userToken, notification, data);
        }

        public async Task SendComment(string userNickname, string targetUserId, string feedEntryId)
        {
            var userToken = await GetUserToken(targetUserId);

            var notification = new Notification
            {
                Title = userNickname,
                Body = "commented your post"
            };

            var data = new Dictionary<string, string>
            {
                {"type", NotificationTypes.CommentNotification},
                {"feedEntryId", feedEntryId}
            };

            await SendToUserInternal(userToken, notification, data);
        }

        public async Task SendFollow(string userNickname, string targetUseId)
        {
            var userToken = await GetUserToken(targetUseId);

            var notification = new Notification
            {
                Title = userNickname,
                Body = "just followed you"
            };
            
            var data = new Dictionary<string, string>
            {
                {"type", NotificationTypes.FollowNotification},
                {"userNickname", userNickname}
            };

            await SendToUserInternal(userToken, notification, data);
        }

        private async Task SendToUserInternal(string userFirebaseToken, Notification notification,
            Dictionary<string, string> data)
        {
            AddRequiredDataProperties(data);

            var message = new Message
            {
                Notification = notification,
                Data = data,
                Token = userFirebaseToken
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Couldn't send notification to {userFirebaseToken}", userFirebaseToken);
            }
        }

        private void AddRequiredDataProperties(Dictionary<string, string> data)
        {
            data["action"] = "FLUTTER_NOTIFICATION_CLICK";
        }

        private Task<string> GetUserToken(string userId)
        {
            return _userRepository.GetFirebaseToken(userId);
        }
    }
}









