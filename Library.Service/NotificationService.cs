using Library.Core;
using Library.Core.Entities;
using Library.Core.Entities.Identity;
using Library.Core.ServiceContract;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public NotificationService(IUnitOfWork unitOfWork , UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<bool> SendNotificationAsync(string userId, string title, string message)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return false;
            var notification = new Notifications
            {
                UserId = userId,
                Title = title,
                Message = message,
            };
            await _unitOfWork.Repository<Notifications>().AddAsync(notification);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IReadOnlyList<Notifications>> GetUserNotificationsAsync(string userEamil)
        {
            var userId = (await _userManager.FindByEmailAsync(userEamil))?.Id;
            return await _unitOfWork.Repository<Notifications>()
                .FindAsync(u=>u.UserId==userId);
        }
        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _unitOfWork.Repository<Notifications>().GetByIdAsync(id);
            if (notification is null)
                return false;
            notification.IsRead = true;
            _unitOfWork.Repository<Notifications>().Update(notification);
           var result = await _unitOfWork.CompleteAsync();
            return result>0;
        }

        public Task<Notifications?> GetByIdAsync(int id)
        {
            var notification = _unitOfWork.Repository<Notifications>().GetByIdAsync(id);
            return notification;
        }
    }
}
