using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IAttendanceRepository Attendances { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IDepartmentRepository Departments { get; private set; }
        public ILeaveRequestRepository LeaveRequests { get; private set; }
        public ISalaryRepository Salaries { get; private set; }
        public ISalaryAdjustmentRepository SalaryAdjustments { get; private set; }
        public IEmployeeDocumentRepository EmployeeDocuments { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IPerformanceReviewRepository PerformanceReviews { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IProjectAssignmentRepository ProjectAssignments { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Attendances = new AttendanceRepository(_context);
            Employees = new EmployeeRepository(_context);
            Departments = new DepartmentRepository(_context);
            LeaveRequests = new LeaveRequestRepository(_context);
            Salaries = new SalaryRepository(_context);
            SalaryAdjustments = new SalaryAdjustmentRepository(_context);
            EmployeeDocuments = new EmployeeDocumentRepository(_context);
            Notifications = new NotificationRepository(_context);
            PerformanceReviews = new PerformanceReviewRepository(_context);
            Projects = new ProjectRepository(_context);
            ProjectAssignments = new ProjectAssignmentRepository(_context);
        }

        public int Complete()
        {
            // Save changes to the database
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            // Dispose of the context if it implements IDisposable
            if (_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this); // Suppress finalization for this object
        }
    }
}
