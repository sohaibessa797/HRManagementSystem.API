using HRManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class PerformanceReview : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [Required]
        [StringLength(100)]
        public string ReviewerName { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
