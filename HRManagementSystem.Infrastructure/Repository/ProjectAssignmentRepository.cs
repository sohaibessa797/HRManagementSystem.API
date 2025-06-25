using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class ProjectAssignmentRepository : GenericRepository<ProjectAssignment>, IProjectAssignmentRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectAssignmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(ProjectAssignment projectAssignment)
        {
            var existingAssignment = _context.ProjectAssignments.Find(projectAssignment.Id);
            if (existingAssignment != null)
            {
                existingAssignment.EmployeeId = projectAssignment.EmployeeId;
                existingAssignment.ProjectId = projectAssignment.ProjectId;
                existingAssignment.AssignedAt = projectAssignment.AssignedAt;
                existingAssignment.CreatedAt = projectAssignment.CreatedAt;
            }
            else
            {
                throw new KeyNotFoundException("Project assignment not found.");
            }
        }
    }
}
