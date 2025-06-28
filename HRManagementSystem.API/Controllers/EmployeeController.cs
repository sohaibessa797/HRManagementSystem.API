using AutoMapper;
using HRManagementSystem.Application.Dtos.Employee;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;


        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        // ===================== GET METHODS =====================

        // GET: api/Employee/all
        // [GET] Get all employees except the current authenticated user (only HR/Admin)
        [HttpGet("all")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> All()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employees = _unitOfWork.Employees.GetAll(
                e => e.ApplicationUserId.ToString() != currentUserId,
                d => d.Department);

            if (!employees.Any())
                return NotFound(new { success = false, message = "No employees found." });            

            var employeeResponses = _mapper.Map<List<EmployeeResponse>>(employees);

            // Fetch and assign roles to each employee
            foreach (var employee in employees)
            {
                var response = employeeResponses.FirstOrDefault(x => x.Id == employee.Id);
                if (response != null)
                    response.Roles = await _userService.GetUserRolesAsync(employee.ApplicationUserId);
            }

            return Ok(new
            {
                success = true,
                message = "All Employees requests retrieved successfully.",
                data = employeeResponses
            });
        }

        // GET: api/Employee/{id}
        // Get employee by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(d => d.Id == id,d=>d.Department,d => d.PerformanceReviews);
            if (employee == null)
                return NotFound(new { success = false, message = $"Employee with ID {id} not found." });

            var response = _mapper.Map<EmployeeResponse>(employee);
            response.Roles = await _userService.GetUserRolesAsync(employee.ApplicationUserId);

            return Ok(new
            {
                success = true,
                data = response
            });
        }

        // GET: api/Employee/profile
        // Get the current authenticated employee's profile
        [HttpGet("profile")]
        [Authorize(Roles = "HR,Admin,Employee")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var employee = _unitOfWork.Employees
                .FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId),
                                e => e.Department,
                                e => e.PerformanceReviews); 

            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }

            var response = _mapper.Map<EmployeeResponse>(employee);
            response.Roles = await _userService.GetUserRolesAsync(employee.ApplicationUserId);

            return Ok(response);
        }

        // ===================== UPDATE METHODS =====================


        // PUT: api/Employee/update
        // Update the current authenticated employee's profile
        [HttpPut("update")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateProfile([FromBody] EmployeeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId),d=>d.Department);
            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }
            _mapper.Map(request, employee);
            _unitOfWork.Employees.Update(employee);
            _unitOfWork.Complete();
            var response = _mapper.Map<EmployeeResponse>(employee);

            response.Roles = await _userService.GetUserRolesAsync(employee.ApplicationUserId);

            return Ok(response);
        }

        // PUT: api/Employee/update/HR/{id}
        // HR or Admin can update an employee profile
        [HttpPut("update/HR/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> EditEmployee(Guid id, [FromBody] UpdateEmployeeByHRRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == id,d=>d.Department);
            if (employee == null)
                return NotFound("Employee not found.");

            _mapper.Map(request, employee);

            _unitOfWork.Employees.Update(employee);
            _unitOfWork.Complete();
            var response = _mapper.Map<EmployeeResponse>(employee);
            response.Roles = await _userService.GetUserRolesAsync(employee.ApplicationUserId);


            return Ok(new
            {
                message = "Employee updated successfully",
                employee = response
            });

        }

        // ===================== DELETE METHODS =====================


        // PATCH: api/Employee/Soft-Delete/{id}
        // Soft delete an employee (Admin only)
        [HttpDelete("Soft-Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult SoftDelete(Guid id)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(d => d.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            _unitOfWork.Employees.SoftDelete(employee);
            _unitOfWork.Complete();
            return NoContent();
        }

    }
}
