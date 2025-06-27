using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.PerformanceReview
{
    public class PerformanceReviewRequest
    {
        public Guid EmployeeId { get; set; }

        public DateTime ReviewDate { get; set; }

        public string ReviewerName { get; set; }

        public string? Comments { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
