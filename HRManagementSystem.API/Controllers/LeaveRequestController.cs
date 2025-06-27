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
        public LeaveRequestController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ===================== GET METHODS =====================

        // GET: api/LeaveRequest/all
        // [GET] Get all leave requests (only for HR/Admin)
        [HttpGet]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetAllRequests()
        {
            var leaveRequests = _unitOfWork.LeaveRequests.GetAll(null, l => l.Employee);

            var result = _mapper.Map<List<LeaveRequestResponse>>(leaveRequests);
            return Ok(new
            {
                success = true,
                message = "All leave requests retrieved successfully.",
                data = result
            });
        }

        // GET: api/LeaveRequest/my-requests
        // [GET] Get current employee's leave requests
        [HttpGet("my-requests")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));

            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found." });

            var leaveRequests = _unitOfWork.LeaveRequests
                .GetAll(l => l.EmployeeId == employee.Id);

            var result = _mapper.Map<List<LeaveRequestResponse>>(leaveRequests);
            return Ok(new
            {
                success = true,
                message = "Your leave requests retrieved successfully.",
                data = result
            });
        }

        // ===================== POST METHODS =====================

        // POST: api/LeaveRequest
        // [POST] Submit a new leave request by employee
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult RequestLeave([FromBody] LeaveRequestRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            if (request.FromDate > request.ToDate)
                return BadRequest(new { success = false, message = "Start date cannot be after end date." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound(new { success = false, message = "Employee not found." });

            var leaveRequest = _mapper.Map<LeaveRequest>(request);
            leaveRequest.EmployeeId = employee.Id;

            _unitOfWork.LeaveRequests.Add(leaveRequest);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Leave request submitted successfully.",
                data = _mapper.Map<LeaveRequestResponse>(leaveRequest)
            });
        }

        // ===================== PATCH METHODS =====================

        // PATCH: api/LeaveRequest/{id}/status
        // [PATCH] Update the status of a leave request (only for HR/Admin)
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult UpdateStatus(Guid id, [FromQuery] LeaveStatus status, [FromQuery] string? note)
        {
            var request = _unitOfWork.LeaveRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound(new { success = false, message = "Leave request not found." });

            request.Status = status;
            request.AdminNote = note;

            _unitOfWork.LeaveRequests.Update(request);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = $"Leave request status updated to {status}."
            });
        }

        // ===================== DELETE METHODS =====================

        // DELETE: api/LeaveRequest/{id}
        // [DELETE] Delete a leave request (only for HR/Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Delete(Guid id)
        {
            var request = _unitOfWork.LeaveRequests.FirstOrDefault(r => r.Id == id);
            if (request == null)
                return NotFound("Leave request not found.");
            _unitOfWork.LeaveRequests.Remove(request);
            _unitOfWork.Complete();
            return Ok(new
            {
                success = true,
                message = "Leave request deleted successfully."
            });
        }
    }
}
