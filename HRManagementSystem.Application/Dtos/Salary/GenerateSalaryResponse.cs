using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class GenerateSalaryResponse
    {
        public decimal BaseSalary { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal FinalAmount { get; set; }
        public string Message { get; set; }
    }
}
