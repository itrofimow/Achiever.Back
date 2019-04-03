using Achiever.Core.Models.User;

namespace Achiever.Core
{
    public interface ICurrentUser
    {
        User User { get; }

        string UserId { get; }
    }
    
    public class CurrentUser : ICurrentUser
    {
        public User User { get; set; }

        public string UserId => User.Id;
    }
}