using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Salary
{
    public class SalaryAdjustmentItemDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public DateTime EffectiveDate { get; set; }

    }
}
