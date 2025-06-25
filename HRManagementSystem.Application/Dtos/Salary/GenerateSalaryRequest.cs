using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class GenerateSalaryRequest
    {
        public Guid EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
