using AutoMapper;
using HRManagementSystem.Application.Dtos.Department;
using HRManagementSystem.Application.Dtos.Employee;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR,Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet("all")]
        public IActionResult All()
        {
            var departments = _unitOfWork.Departments.GetAll();
            if (departments == null)
            {
                return NotFound();
            }
            // Include related employees data using a custom mapping helper
            //var departmentsDto = MappingHelper.MapToDtoList(departments);

            // Include related employees data using AutoMapper
            var departmentsDto = _mapper.Map<List<DepartmentResponse>>(departments);


            return Ok(departmentsDto);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var department = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }
            var departmentDto = _mapper.Map<DepartmentResponse>(department);
            return Ok(departmentDto);
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody] DepartmentRequest dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var department = _mapper.Map<Department>(dto);

            _unitOfWork.Departments.Add(department);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, _mapper.Map<DepartmentResponse>(department));
        }
        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, [FromBody] DepartmentRequest dto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDepartment = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (existingDepartment == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            _mapper.Map(dto, existingDepartment);
            _unitOfWork.Departments.Update(existingDepartment);
            _unitOfWork.Complete();
            return NoContent();
        }
        [HttpPatch("Soft-Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult SoftDeleteDepartment(Guid id)
        {
            var department = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            _unitOfWork.Departments.SoftDelete(department);
            _unitOfWork.Complete();
            return NoContent();
        }
    }
}
