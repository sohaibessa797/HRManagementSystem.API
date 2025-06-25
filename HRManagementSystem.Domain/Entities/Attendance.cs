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
    public class Attendance : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    }
}
