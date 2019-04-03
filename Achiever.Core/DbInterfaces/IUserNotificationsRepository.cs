using System.Collections.Generic;
using System.Threading.Tasks;
using Achiever.Core.Models;

namespace Achiever.Core.DbInterfaces
{
    public interface IUserNotificationsRepository
    {
        Task AddNotification(UserNotification notification);

        Task<List<UserNotification>> GetAllForUser(string userId);
    }
}