using System;
using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models
{
    [MongoEntity("AcquiredAchievements")]
    public class AcquiredAchievement
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string AchievementId { get; set; }

        public string OwnerId { get; set; }

        public DateTime AcquiredAt { get; set; }
    }
}
