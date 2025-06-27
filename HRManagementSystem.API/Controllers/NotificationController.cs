using AutoMapper;
using HRManagementSystem.API.Hubs;
using HRManagementSystem.Application.Dtos.Notification;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;


        public NotificationController(IUnitOfWork unitOfWork,IMapper mapper,IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found.");

            var applicationUserId = employee.ApplicationUserId;

            var notification = _mapper.Map<Notification>(request);
            _unitOfWork.Notifications.Add(notification);
            _unitOfWork.Complete();

            await _hubContext.Clients
              .User(applicationUserId.ToString())
              .SendAsync("ReceiveNotification", new
              {
                  Id = notification.Id,
                  Message = notification.Message,
                  IsRead = notification.IsRead,
                  CreatedAt = notification.CreatedAt
              });

            return Ok(new
            {
                message = "Notification sent successfully.",
                data = _mapper.Map<NotificationResponse>(notification)
            });
        }

        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetDocumentsByEmployee(Guid employeeId)
        {
            var notification = _unitOfWork.Notifications
                .GetAll(d => d.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedAt);


            if (!notification.Any())
                return NotFound("No documents found for this employee.");

            var response = _mapper.Map<List<NotificationResponse>>(notification);

            return Ok(response);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employee = _unitOfWork.Employees
                .FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));

            if (employee == null)
                return Unauthorized();

            var notifications = _unitOfWork.Notifications
             .GetAll(n => n.EmployeeId == employee.Id)
             .OrderByDescending(n => n.CreatedAt)
             .ToList();

            var result = _mapper.Map<List<NotificationResponse>>(notifications);
            return Ok(result);
        }

        [HttpPatch("mark-read/{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult MarkAsRead(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employee = _unitOfWork.Employees
                .FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return Unauthorized();


            var notification = _unitOfWork.Notifications
                .FirstOrDefault(n => n.Id == id && n.EmployeeId == employee.Id);

            if (notification == null)
                return NotFound("Notification not found.");

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            _unitOfWork.Complete();

            return Ok(new { message = "Notification marked as read." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Delete(Guid id)
        {
            var notification = _unitOfWork.Notifications.FirstOrDefault(n => n.Id == id);
            if (notification == null)
                return NotFound("Notification not found.");

            _unitOfWork.Notifications.Remove(notification);
            _unitOfWork.Complete();

            return Ok(new { message = "Notification deleted successfully." });
        }
    }
}
