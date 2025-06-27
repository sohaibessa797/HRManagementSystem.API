using AutoMapper;
using HRManagementSystem.Application.Dtos.Project;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "HR,Admin")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProjectController(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork ;
            _mapper = mapper ; 
        }

        // ===================== GET METHODS =====================


        // GET: api/Project/all
        [HttpGet("all")]
        public IActionResult GetAllProjects()
        {
            var projects = _unitOfWork.Projects.GetAll().ToList();
            var response = _mapper.Map<List<ProjectResponse>>(projects);
            return Ok(new
            {
                success = true,
                message = "All projects retrieved successfully.",
                data = response
            });
        }

        // GET: api/Project/my-projects
        [HttpGet("my-projects")]
        public IActionResult GetMyProjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = _unitOfWork.Employees.FirstOrDefault(e => e.ApplicationUserId == Guid.Parse(userId));

            if (employee == null)
                return Unauthorized(new { success = false, message = "Employee not found." });

            var assignments = _unitOfWork.ProjectAssignments
                .GetAll(pa => pa.EmployeeId == employee.Id, pa => pa.Project)
                .ToList();

            var result = _mapper.Map<List<ProjectAssignmentResponse>>(assignments);
            return Ok(new
            {
                success = true,
                message = "Your projects retrieved successfully.",
                data = result
            });
        }

        // GET: api/Project/{projectId}/employees
        [HttpGet("{projectId}/employees")]
        [Authorize(Roles = "HR,Admin")]
        public IActionResult GetEmployeesInProject(Guid projectId)
        {
            var assignments = _unitOfWork.ProjectAssignments
                .GetAll(pa => pa.ProjectId == projectId, pa => pa.Employee, p => p.Project)
                .ToList();
            if (assignments == null)
                return NotFound(new { success = false, message = "Project Assignments not found." });

            var result = _mapper.Map<List<ProjectAssignmentResponse>>(assignments);
            return Ok(new
            {
                success = true,
                message = "Employees in project retrieved successfully.",
                data = result
            });
        }

        // ===================== POST METHODS =====================

        // POST: api/Project
        [HttpPost]
        public IActionResult CreateProject([FromBody] ProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            var project = _mapper.Map<Project>(request);
            _unitOfWork.Projects.Add(project);
            _unitOfWork.Complete();

            var response = _mapper.Map<ProjectResponse>(project);
            return Ok(new
            {
                success = true,
                message = "Project created successfully.",
                data = response
            });
        }

        // POST: api/Project/assign
        [HttpPost("assign")]
        public IActionResult AssignEmployeesToProject([FromBody] AssignEmployeeToProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            var project = _unitOfWork.Projects.FirstOrDefault(p => p.Id == request.ProjectId);
            if (project == null)
                return NotFound(new { success = false, message = "Project not found." });

            var addedAssignments = new List<ProjectAssignment>();

            foreach (var employeeId in request.EmployeeIds)
            {
                var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee == null)
                    continue;

                var alreadyAssigned = _unitOfWork.ProjectAssignments
                    .Any(pa => pa.ProjectId == request.ProjectId && pa.EmployeeId == employeeId);

                if (alreadyAssigned)
                    continue;

                var assignment = new ProjectAssignment
                {
                    ProjectId = request.ProjectId,
                    EmployeeId = employeeId,
                    AssignedAt = DateTime.UtcNow
                };

                _unitOfWork.ProjectAssignments.Add(assignment);
                addedAssignments.Add(assignment);
            }

            _unitOfWork.Complete();
            var response = _mapper.Map<List<ProjectAssignmentResponse>>(addedAssignments);
            return Ok(new
            {
                success = true,
                message = "Employees assigned to project successfully.",
                data = response
            });
        }

        // ===================== PUT METHODS =====================

        // PUT: api/Project/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult UpdateProject(Guid id, [FromBody] ProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            var existingProject = _unitOfWork.Projects.FirstOrDefault(p => p.Id == id);
            if (existingProject == null)
                return NotFound(new { success = false, message = "Project not found." });

            _mapper.Map(request, existingProject);

            _unitOfWork.Projects.Update(existingProject);
            _unitOfWork.Complete();

            var response = _mapper.Map<ProjectResponse>(existingProject);
            return Ok(new
            {
                success = true,
                message = "Project updated successfully.",
                data = response
            });
        }

        // PUT: api/Project/update-assignments
        [HttpPut("update-assignments")]
        public IActionResult UpdateProjectAssignments([FromBody] AssignEmployeeToProjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid input.", errors = ModelState });

            var project = _unitOfWork.Projects.FirstOrDefault(p => p.Id == request.ProjectId);
            if (project == null)
                return NotFound(new { success = false, message = "Project not found." });

            var currentAssignments = _unitOfWork.ProjectAssignments
                .GetAll(pa => pa.ProjectId == request.ProjectId)
                .ToList();

            var currentEmployeeIds = currentAssignments.Select(pa => pa.EmployeeId).ToList();

            var newEmployeeIds = request.EmployeeIds.Except(currentEmployeeIds).ToList();

            var removedEmployeeIds = currentEmployeeIds.Except(request.EmployeeIds).ToList();

            var addedAssignments = new List<ProjectAssignment>();

            // Remove old
            foreach (var assignment in currentAssignments.Where(a => removedEmployeeIds.Contains(a.EmployeeId)))
            {
                _unitOfWork.ProjectAssignments.Remove(assignment);
            }

            // Add new
            foreach (var employeeId in newEmployeeIds)
            {
                var employee = _unitOfWork.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee == null)
                    continue;

                var assignment = new ProjectAssignment
                {
                    ProjectId = request.ProjectId,
                    EmployeeId = employeeId,
                    AssignedAt = DateTime.UtcNow
                };

                _unitOfWork.ProjectAssignments.Add(assignment);
                addedAssignments.Add(assignment);

            }

            _unitOfWork.Complete();
            var response = _mapper.Map<List<ProjectAssignmentResponse>>(addedAssignments);

            return Ok(new
            {
                success = true,
                message = "Project assignments updated successfully.",
                data = response
            });
        }

        // DELETE: api/Project/{id}
        // [DELETE] Delete project and related assignments
        [HttpPatch("{id}")]
        public IActionResult DeleteProject(Guid id)
        {
            var project = _unitOfWork.Projects.FirstOrDefault(p => p.Id == id);
            if (project == null)
                return NotFound(new { success = false, message = "Project not found." });

            _unitOfWork.Projects.SoftDelete(project);
            _unitOfWork.Complete();

            return Ok(new
            {
                success = true,
                message = "Project and related assignments deleted successfully (soft delete)."
            });
        }

    }
}
