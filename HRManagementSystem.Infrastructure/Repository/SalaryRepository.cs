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
    public class SalaryRepository : GenericRepository<Salary> , ISalaryRepository
    {
        private readonly ApplicationDbContext _context;
        public SalaryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Salary salary)
        {
            var existingSalary = _context.Salaries.Find(salary.Id);
            if (existingSalary != null)
            {
                existingSalary.EmployeeId = salary.EmployeeId;
                existingSalary.Amount = salary.Amount;
                existingSalary.Month = salary.Month;
                existingSalary.Year = salary.Year;
                existingSalary.IsPaid = salary.IsPaid;
                existingSalary.CreatedAt = salary.CreatedAt;
            }
            else
            {
                throw new KeyNotFoundException("Salary not found.");
            }
        }
    }
}
