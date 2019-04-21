using System.Collections.Generic;
using System.Linq;
using Achiever.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Achiever.Core.Models.User
{
    [MongoEntity("Users")]
    public class User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [JsonIgnore]
        public string FirebaseToken { get; set; }

        public string Nickname { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string About { get; set; }

        public string DisplayName { get; set; }

        public string ProfileImagePath { get; set; }

        public UserStats Stats => new UserStats
        {
            Followers = FollowersIds.Count,
            Following = FollowingIds.Count
        };

        public HashSet<string> FollowersIds { get; set; } = new HashSet<string>();

        public HashSet<string> FollowingIds { get; set; } = new HashSet<string>();

        public List<string> GetAllFollowers()
        {
            var allFollowersList = FollowersIds.ToList();
            allFollowersList.Add(Id);

            return allFollowersList;
        }
    }
    
    public class UserStats
    {
        public int Following { get; set; }

        public int Followers { get; set; }

        public int AchievementsCount { get; set; }
    }
}