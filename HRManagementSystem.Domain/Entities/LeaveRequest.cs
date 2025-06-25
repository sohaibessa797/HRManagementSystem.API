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
    public class LeaveRequest : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [StringLength(250)]
        public string Reason { get; set; }

        public LeaveType LeaveType { get; set; }

        [MaxLength(250)]
        public string? AdminNote { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    }
}
