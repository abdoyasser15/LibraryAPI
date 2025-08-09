using Library.Core.Entities;
using Library.Core.RepositoryContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface INotificationService 
    {
        Task<bool> SendNotificationAsync(string userId, string title, string message);
        Task<IReadOnlyList<Notifications>> GetUserNotificationsAsync(string userId);
        Task<bool> MarkAsReadAsync(int id);
        Task<Notifications?> GetByIdAsync(int id);
    }
}
