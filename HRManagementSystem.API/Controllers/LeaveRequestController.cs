using AutoMapper;
using HRManagementSystem.Application.Dtos.LeaveRequest;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public LeaveRequestController( IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult RequestLeave([FromBody] LeaveRequestRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var leaveRequest = _mapper.Map<LeaveRequest>(dto);
            leaveRequest.EmployeeId = employee.Id;

            _unitOfWork.LeaveRequests.Add(leaveRequest);
            _unitOfWork.Complete();

            return Ok(new { message = "Leave request submitted successfully." });
        }

        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetAllRequests()
        {
            var leaveRequests = _unitOfWork.LeaveRequests.GetAll(null, l => l.Employee);
            var result = _mapper.Map<List<LeaveRequestResponse>>(leaveRequests);
            return Ok(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult UpdateStatus(Guid id, [FromQuery] LeaveStatus status, [FromQuery] string? note)
        {
            var request = _unitOfWork.LeaveRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound("Leave request not found.");

            request.Status = status;
            request.AdminNote = note;

            _unitOfWork.LeaveRequests.Update(request);
            _unitOfWork.Complete();

            return Ok(new { message = $"Leave request marked as {status}" });
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));

            if (employee == null)
                return NotFound("Employee not found.");

            var leaveRequests = _unitOfWork.LeaveRequests
                .GetAll(l => l.EmployeeId == employee.Id);

            var result = _mapper.Map<List<LeaveRequestResponse>>(leaveRequests);
            return Ok(result);
        }
    }
}
