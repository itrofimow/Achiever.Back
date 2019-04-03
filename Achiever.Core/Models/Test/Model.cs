using System.Collections.Generic;
using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Achiever.Core.Models.Test
{
    [MongoEntity("ModelA")]
    public class ModelA
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ModelBId { get; set; }

        [BsonIgnore]
        public ModelB B { get; set; }
    }

    [MongoEntity("ModelB")]
    public class ModelB
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class ModelAWithBs
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string ModelBId { get; set; }

        public IEnumerable<ModelB> Bs { get; set; }
    }
}