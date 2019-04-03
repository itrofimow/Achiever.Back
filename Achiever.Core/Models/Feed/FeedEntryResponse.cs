namespace Achiever.Core.Models.Feed
{
    public class FeedEntryResponse
    {
        public string AuthorProfileImagePath { get; set; }

        public string AuthorNickname { get; set; }

        public FeedEntry Entry { get; set; }

        public bool IsLiked { get; set; }
    }
}