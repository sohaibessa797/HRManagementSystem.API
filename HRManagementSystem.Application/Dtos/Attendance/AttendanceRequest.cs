using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Attendance
{
    public class AttendanceRequest
    {
        public Guid EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
