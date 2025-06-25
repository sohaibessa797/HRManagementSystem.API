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

        [HttpPost("check-in")]
        [Authorize(Roles = "Employee")]
        public IActionResult CheckIn([FromBody] AttendanceCheckInRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var existing = _unitOfWork.Attendances.FirstOrDefault(
                a => a.EmployeeId == employee.Id && a.Date.Date == dto.Date.Date);

            if (existing != null)
                return BadRequest("You already checked in today.");

            var attendance = new Attendance
            {
                EmployeeId = employee.Id,
                Date = dto.Date.Date,
                CheckIn = dto.CheckIn,
                Status = dto.CheckIn > new TimeSpan(9, 0, 0) 
                ? AttendanceStatus.Late 
                : AttendanceStatus.Present
            };

            _unitOfWork.Attendances.Add(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Check-in recorded." });
        }

        [HttpPatch("check-out")]
        [Authorize(Roles = "Employee")]
        public IActionResult CheckOut([FromBody] AttendanceCheckOutRequest dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));
            if (employee == null)
                return NotFound("Employee not found.");

            var attendance = _unitOfWork.Attendances.FirstOrDefault(
                a => a.EmployeeId == employee.Id && a.Date.Date == dto.Date.Date);

            if (attendance == null)
                return NotFound("You must check in first.");

            attendance.CheckOut = dto.CheckOut;
            
            if (dto.CheckOut < new TimeSpan(15, 0, 0))
            {
                attendance.Status = AttendanceStatus.Leave;
            }

            _unitOfWork.Attendances.Update(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Check-out recorded." });
        }

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

        [HttpGet("by-employee/{employeeId}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetAttendanceByEmployee(Guid employeeId)
        {
            var records = _unitOfWork.Attendances.GetAll(a => a.EmployeeId == employeeId, a => a.Employee);
            var result = _mapper.Map<List<AttendanceResponse>>(records);

            return Ok(result.OrderByDescending(r => r.Date));
        }

        [HttpPost("create")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Create([FromBody] AttendanceRequest dto)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == dto.EmployeeId);
            if (employee == null)
                return NotFound("Employee not found.");

            var attendance = _mapper.Map<Attendance>(dto);
            _unitOfWork.Attendances.Add(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Attendance record added manually." });
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult Update(Guid id, [FromBody] AttendanceRequest dto)
        {
            var attendance = _unitOfWork.Attendances.FirstOrDefault(a => a.Id == id);
            if (attendance == null)
                return NotFound("Attendance not found.");

            _mapper.Map(dto, attendance);
            _unitOfWork.Attendances.Update(attendance);
            _unitOfWork.Complete();

            return Ok(new { message = "Attendance updated successfully." });
        }

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
