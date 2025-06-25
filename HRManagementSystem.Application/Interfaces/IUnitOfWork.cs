using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        IAttendanceRepository Attendances { get; }
        ILeaveRequestRepository LeaveRequests { get; }
        ISalaryRepository Salaries { get; }
        ISalaryAdjustmentRepository SalaryAdjustments { get; }
        IEmployeeDocumentRepository EmployeeDocuments { get; }
        INotificationRepository Notifications { get; }
        IPerformanceReviewRepository PerformanceReviews { get; }
        IProjectRepository Projects { get; }
        IProjectAssignmentRepository ProjectAssignments { get; }
        int Complete();
        void Dispose();
    }
}
