using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class PerformanceReviewRepository : GenericRepository<PerformanceReview>, IPerformanceReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public PerformanceReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(PerformanceReview performanceReview)
        {
            var existingReview = _context.PerformanceReviews.Find(performanceReview.Id);
            if (existingReview != null)
            {
                existingReview.EmployeeId = performanceReview.EmployeeId;
                existingReview.ReviewDate = performanceReview.ReviewDate;
                existingReview.ReviewerName = performanceReview.ReviewerName;
                existingReview.Rating = performanceReview.Rating;
                existingReview.Comments = performanceReview.Comments;
            }
            else
            {
                throw new ArgumentException("Performance review not found.");
            }
        }
    }
}
