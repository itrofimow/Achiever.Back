using System.Collections.Generic;
using System.Threading.Tasks;

namespace Achiever.Core.Notifications
{
    public interface INotificationSender
    {
        Task SendLike(string userNickname, string targetUserId, string feedEntryId);
        
        Task SendComment(string userNickname, string targetUserId, string feedEntryId);

        Task SendFollow(string userNickname, string targetUserId);
    }
}