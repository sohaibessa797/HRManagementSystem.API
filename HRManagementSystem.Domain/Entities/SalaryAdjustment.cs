using HRManagementSystem.Domain.Common;
using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class SalaryAdjustment : BaseEntity
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public decimal Amount { get; set; } 

        [Required]
        public AdjustmentType Type { get; set; }

        [StringLength(250)]
        public string Reason { get; set; }

        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
    }
}
