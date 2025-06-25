using AutoMapper;
using HRManagementSystem.Application.Dtos.Employee;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("all")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> All()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employees = _unitOfWork.Employees.GetAll(
                e => e.ApplicationUserId.ToString() != currentUserId,
                d => d.Department);

            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found.");
            }

            var employeeDtos = _mapper.Map<List<EmployeeResponse>>(employees);

            foreach (var employee in employees)
            {
                var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var dto = employeeDtos.FirstOrDefault(x => x.Id == employee.Id);
                    if (dto != null)
                        dto.Roles = roles.ToList();
                }
            }

            return Ok(employeeDtos);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var employee = _unitOfWork.Employees.FirstOrDefault(d => d.Id == id,d=>d.Department);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            var employeeDto = _mapper.Map<EmployeeResponse>(employee);
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                employeeDto.Roles = roles.ToList(); 
            }

            return Ok(employeeDto);
        }


        [HttpGet("profile")]
        [Authorize(Roles = "HR,Admin,Employee")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId), e => e.Department);
            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }
            var employeeDto = _mapper.Map<EmployeeResponse>(employee);
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                employeeDto.Roles = roles.ToList();
            }

            return Ok(employeeDto);
        }
        [HttpPut("update")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> UpdateProfile([FromBody] EmployeeRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
            _mapper.Map(dto, employee);
            _unitOfWork.Employees.Update(employee);
            _unitOfWork.Complete();
            var response = _mapper.Map<EmployeeResponse>(employee);
            
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                response.Roles = roles.ToList();
            }
            return Ok(response);
        }

        [HttpPut("update/HR/{id}")]
        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> EditEmployee(Guid id, [FromBody] UpdateEmployeeByHRRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == id,d=>d.Department);
            if (employee == null)
                return NotFound("Employee not found.");

            _mapper.Map(dto, employee);

            _unitOfWork.Employees.Update(employee);
            _unitOfWork.Complete();
            var response = _mapper.Map<EmployeeResponse>(employee);
            var user = await _userManager.FindByIdAsync(employee.ApplicationUserId.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                response.Roles = roles.ToList();
            }
            return Ok(new { message = "Employee updated successfully", 
                employee = response
            });
        }

        [HttpPatch("Soft-Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult SoftDeleteEmployee(Guid id)
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
