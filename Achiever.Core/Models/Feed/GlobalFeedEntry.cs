using System;
using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models.Feed
{
    [MongoEntity("GlobalFeed")]
    public class GlobalFeedEntry
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FeedOwnerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FeedEntryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime InsertedAt { get; set; }
    }
}