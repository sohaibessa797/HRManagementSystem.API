using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class CreateSalaryAdjustmentRequest
    {
        public Guid EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public AdjustmentType Type { get; set; } // Bonus / Deduction
        public string Reason { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
