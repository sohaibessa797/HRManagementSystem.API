using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class SalaryDetailsResponse
    {
        public string EmployeeName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal TotalBonus { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal FinalAmount { get; set; }
        public bool IsPaid { get; set; }

        public List<SalaryAdjustmentItemDto> Bonuses { get; set; }
        public List<SalaryAdjustmentItemDto> Deductions { get; set; }
    }
}
