using System;
using System.Collections.Generic;
using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models.Feed
{
    [MongoEntity("FeedEntries")]
    public class FeedEntry
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AchievementId { get; set; }

        [BsonIgnore] public Achievement Achievement { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Comment { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public List<string> Likes { get; set; } = new List<string>();

        [BsonIgnore] public int LikesCount { get; set; }
        
        
        public List<FeedEntryComment> Comments { get; set; } = new List<FeedEntryComment>();

        [BsonIgnore] public int CommentsCount { get; set; }
    }

    public class FeedEntryComment
    {
        public string Text { get; set; }

        public string AuthorId { get; set; }

        [BsonIgnore] public string AuthorProfileImage { get; set; }

        [BsonIgnore] public string AuthorNickname { get; set; }
    }
}





