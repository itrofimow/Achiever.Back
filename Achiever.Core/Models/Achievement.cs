using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models
{
    [MongoEntity("Achievements")]
    public class Achievement
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public bool HasDefaultBackground { get; set; }

        public string DefaultBackgroundColor { get; set; }

        public ImageInfo BackgroundImage { get; set; }

        public ImageInfo FrontImage { get; set; }

        public ImageInfo BigImage { get; set; }

        
        [BsonRepresentation(BsonType.ObjectId)]
        public string AchievementCategoryId { get; set; }

        [BsonIgnore]
        public AchievementCategory Category { get; set; }
    }

    public class ImageInfo
    {
        public string ImagePath { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}