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
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;
        public LeaveRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(LeaveRequest leaveRequest)
        {
            var existingLeaveRequest = _context.LeaveRequests.Find(leaveRequest.Id);
            if (existingLeaveRequest != null)
            {
                existingLeaveRequest.EmployeeId = leaveRequest.EmployeeId;
                existingLeaveRequest.FromDate = leaveRequest.FromDate;
                existingLeaveRequest.ToDate = leaveRequest.ToDate;
                existingLeaveRequest.Reason = leaveRequest.Reason;
                existingLeaveRequest.Status = leaveRequest.Status;
                existingLeaveRequest.CreatedAt = existingLeaveRequest.CreatedAt; // Keep the original creation date
            }
            else
            {
                throw new ArgumentException("Leave request not found.");
            }
        }
    }
}
