using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.PerformanceReview
{
    public class PerformanceReviewResponse
    {
        public Guid Id { get; set; }

        public DateTime ReviewDate { get; set; }

        public string ReviewerName { get; set; }

        public string? Comments { get; set; }

        public int Rating { get; set; }
    }
}
