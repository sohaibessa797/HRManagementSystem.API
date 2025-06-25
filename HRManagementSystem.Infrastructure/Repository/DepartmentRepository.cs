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
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Department department)
        {
            var existingDepartment = _context.Departments.Find(department.Id);
            if (existingDepartment != null)
            {
                existingDepartment.Name = department.Name;
                existingDepartment.Description = department.Description;
                existingDepartment.CreatedAt = existingDepartment.CreatedAt; // Keep the original creation date
                _context.Departments.Update(existingDepartment);
            }
            else
            {
                throw new ArgumentException("Department not found.");
            }
        }
    }
}
