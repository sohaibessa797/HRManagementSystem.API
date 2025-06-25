using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Attendance
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public string Status { get; set; }
        public string EmployeeName { get; set; }
    }
}
