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
    public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Attendance attendance)
        {
            var existingAttendance = _context.Attendances.Find(attendance.Id);
            if (existingAttendance != null)
            {
                existingAttendance.EmployeeId = attendance.EmployeeId;
                existingAttendance.Date = attendance.Date;
                existingAttendance.Status = attendance.Status;
                existingAttendance.CheckIn = attendance.CheckIn;
                existingAttendance.CheckOut = attendance.CheckOut;
                attendance.CreatedAt = existingAttendance.CreatedAt;
            }
            else
            {
                throw new ArgumentException("Attendance record not found.");
            }
        }
    }
}
