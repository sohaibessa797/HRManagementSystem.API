using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IPerformanceReviewRepository : IGenericRepository<PerformanceReview>
    {
        void Update(PerformanceReview performanceReview);
    }
}
