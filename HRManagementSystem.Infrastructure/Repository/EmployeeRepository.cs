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
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Employee employee)
        {
            var existingEmployee = _context.Employees.Find(employee.Id);
            if (existingEmployee != null)
            {
                existingEmployee.FullName = employee.FullName;
                existingEmployee.ProfilePictureUrl = employee.ProfilePictureUrl;
                existingEmployee.Email = employee.Email;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.DepartmentId = employee.DepartmentId;
                existingEmployee.NationalId = employee.NationalId;
                existingEmployee.JobTitle = employee.JobTitle;
                existingEmployee.IsActive = employee.IsActive;
                existingEmployee.DateOfBirth = employee.DateOfBirth;
                existingEmployee.HireDate = employee.HireDate;
                existingEmployee.Address = employee.Address;
                existingEmployee.Salary = employee.Salary;
                existingEmployee.CreatedAt = existingEmployee.CreatedAt; // Keep the original creation date
            }
            else
            {
                throw new ArgumentException("Employee not found.");
            }
        }
    }
}
