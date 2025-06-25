using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Notification notification)
        {
            var existingNotification = _context.Notifications.FirstOrDefault(n => n.Id == notification.Id);
            if (existingNotification != null)
            {
                existingNotification.EmployeeId = notification.EmployeeId;
                existingNotification.Message = notification.Message;
                existingNotification.IsRead = notification.IsRead;
                existingNotification.CreatedAt = notification.CreatedAt;
            }
            else
            {
                throw new ArgumentException("Notification not found");
            }
        }
    }
}
