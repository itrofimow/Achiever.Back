using System;
using Achiever.Common;
using Achiever.Core.Models.Feed;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models
{
    [MongoEntity("UserNotifications")]
    public class UserNotification
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public UserNotificationType NotificationType { get; set; }

        public string UserId { get; set; }

        
        public string AuthorId { get; set; }

        [BsonIgnore]
        public User.User Author { get; set; }
        

        public string Text { get; set; }
        

        public string AchievementId { get; set; }

        [BsonIgnore]
        public Achievement Achievement { get; set; }

        
        public string FeedEntryId { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum UserNotificationType
    {
        Like = 0,
        
        Comment = 1,
        
        Follow = 2
    }
}