using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class SalaryListResponse
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
        public decimal BaseSalary { get; set; } 
        public bool IsPaid { get; set; }
    }
}
