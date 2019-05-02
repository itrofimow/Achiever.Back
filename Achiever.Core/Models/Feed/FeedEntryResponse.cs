
using System.Collections.Generic;

namespace Achiever.Core.Models.Feed
{
    public class CoreUserDto
    {
        public User.User User { get; set; }

        public bool Following { get; set; }    
    }
    
    public class FeedEntryResponse
    {
        public string AuthorProfileImagePath { get; set; }

        public string AuthorNickname { get; set; }

        public FeedEntry Entry { get; set; }

        public bool IsLiked { get; set; }
    }
}