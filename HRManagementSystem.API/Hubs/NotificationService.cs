using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace HRManagementSystem.API.Hubs
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Guid employeeId, string message)
        {
            var notification = new Notification
            {
                EmployeeId = employeeId,
                Message = message,
                IsRead = false
            };

            _unitOfWork.Notifications.Add(notification);
            _unitOfWork.Complete();

            await _hubContext.Clients
                .Group(employeeId.ToString()) 
                .SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Message,
                    notification.IsRead,
                    CreatedAt = notification.CreatedAt
                });
        }
    }
}
