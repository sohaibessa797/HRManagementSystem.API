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

        // ===================== GET METHODS =====================

        // GET: api/Department/all
        // [GET] Get all departments in the system.
        [HttpGet("all")]
        public IActionResult All()
        {
            var departments = _unitOfWork.Departments.GetAll();
            if (!departments.Any()) 
                return NotFound("No departments found.");

            
            // Include related employees data using AutoMapper
            var response = _mapper.Map<List<DepartmentResponse>>(departments);


            return Ok(response);
        }
        // GET: api/Department/{id}
        // [GET] Get a specific department by ID.
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var department = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }
            var response = _mapper.Map<DepartmentResponse>(department);
            return Ok(response);
        }

        // ===================== POST METHODS =====================

        // POST: api/Department/create
        // [POST] Create a new department.
        [HttpPost("create")]
        public IActionResult Create([FromBody] DepartmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = _unitOfWork.Departments.Any(d => d.Name == request.Name);
            if (exists)
                return BadRequest("A department with the same name already exists.");

            var department = _mapper.Map<Department>(request);

            _unitOfWork.Departments.Add(department);
            _unitOfWork.Complete();
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, _mapper.Map<DepartmentResponse>(department));
        }

        // ===================== PUT METHODS =====================

        // PUT: api/Department/update/{id}
        // [PUT] Update an existing department by ID.
        [HttpPut("update/{id}")]
        public IActionResult Update(Guid id, [FromBody] DepartmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingDepartment = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (existingDepartment == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }

            _mapper.Map(request, existingDepartment);
            _unitOfWork.Departments.Update(existingDepartment);
            _unitOfWork.Complete();
            return Ok(new { message = "Department updated successfully" });
        }

        // ===================== DELETE METHODS =====================

        // DELETE: api/Department/delete/{id}
        // [DELETE] Delete an existing department by ID.
        [HttpPatch("Soft-Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult SoftDelete(Guid id)
        {
            var department = _unitOfWork.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null)
            {
                return NotFound();
            }
            _unitOfWork.Departments.SoftDelete(department);
            _unitOfWork.Complete();
            return Ok(new { message = "Department Deleted successfully" });
        }
    }
}
