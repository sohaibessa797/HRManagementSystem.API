using AutoMapper;
using HRManagementSystem.Application.Dtos;
using HRManagementSystem.Application.Dtos.Attendance;
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
    public class AttendanceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AttendanceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ========================= EMPLOYEE ACTIONS =========================

        // Post: api/Attendance/check-in
        // [POST] Check in by employee for a specific date.
        // Status is set to Late or Present based on time.
        [HttpPost("check-in")]
        [Authorize(Roles = "Employee")]
        public IActionResult CheckIn([FromBody] AttendanceCheckInRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var existing = _unitOfWork.Attendances.FirstOrDefault(
                a => a.EmployeeId == employee.Id && a.Date.Date == request.Date.Date);

            if (existing != null)
                return BadRequest("You already checked in today.");

            var attendance = new Attendance
            {
                EmployeeId = employee.Id,
                Date = request.Date.Date,
                CheckIn = request.CheckIn,
                Status = request.CheckIn > new TimeSpan(9, 0, 0) 
                ? AttendanceStatus.Late 
                : AttendanceStatus.Present
            };

            _unitOfWork.Attendances.Add(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Check-in recorded." });
        }

        // Post: api/Attendance/check-out
        // [POST] Check out by employee for a specific date.
        // If check-out time is before 3 PM, status is set to Leave.
        [HttpPatch("check-out")]
        [Authorize(Roles = "Employee")]
        public IActionResult CheckOut([FromBody] AttendanceCheckOutRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var attendance = _unitOfWork.Attendances.FirstOrDefault(
                a => a.EmployeeId == employee.Id && a.Date.Date == request.Date.Date);

            if (attendance == null)
                return NotFound("You must check in first.");

            attendance.CheckOut = request.CheckOut;
            
            if (request.CheckOut < new TimeSpan(15, 0, 0))
            {
                attendance.Status = AttendanceStatus.Leave;
            }

            _unitOfWork.Attendances.Update(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Check-out recorded." });
        }

        // ========================= GET METHODS =========================

        // Get: api/Attendance/my
        // [GET] Get attendance records for the current employee.
        [HttpGet("my")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyAttendance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var records = _unitOfWork.Attendances.GetAll(a => a.EmployeeId == employee.Id);
            var result = _mapper.Map<List<AttendanceResponse>>(records);

            return Ok(result.OrderByDescending(r => r.Date));
        }

        // Get: api/Attendance/by-employee/{employeeId}
        // [GET] Get attendance records for a specific employee by ID.
        [HttpGet("by-employee/{employeeId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetAttendanceByEmployee(Guid employeeId)
        {
            var records = _unitOfWork.Attendances.GetAll(a => a.EmployeeId == employeeId, a => a.Employee);
            var result = _mapper.Map<List<AttendanceResponse>>(records);

            return Ok(result.OrderByDescending(r => r.Date));
        }

        // ========================= Post METHODS =========================

        // Post: api/Attendance/create
        // [POST] Create a manual attendance record for an employee.
        [HttpPost("create")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Create([FromBody] AttendanceRequest request)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found.");

            var attendance = _mapper.Map<Attendance>(request);
            _unitOfWork.Attendances.Add(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Attendance record added manually." });
        }

        // ========================= PUT METHODS =========================

        // Put: api/Attendance/update/{id}
        // [PUT] Update an existing attendance record by ID.
        [HttpPut("update/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Update(Guid id, [FromBody] AttendanceRequest request)
        {
            var attendance = _unitOfWork.Attendances.FirstOrDefault(a => a.Id == id);
            if (attendance == null)
                return NotFound("Attendance not found.");

            _mapper.Map(request, attendance);
            _unitOfWork.Attendances.Update(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Attendance updated successfully." });
        }

        // ========================= DELETE METHODS =========================

        // Delete: api/Attendance/delete/{id}
        // [DELETE] Delete an attendance record by ID.
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Delete(Guid id)
        {
            var attendance = _unitOfWork.Attendances.FirstOrDefault(a => a.Id == id);
            if (attendance == null)
                return NotFound("Attendance not found.");

            _unitOfWork.Attendances.Remove(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Attendance deleted successfully." });
        }
    }
}
