using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models
{
    [MongoEntity("AchievementCategories")]
    public class AchievementCategory
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public int MaskHeight { get; set; }

        public string MaskImagePath { get; set; }
    }
}