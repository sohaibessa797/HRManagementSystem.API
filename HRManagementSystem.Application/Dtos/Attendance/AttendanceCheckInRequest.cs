using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Attendance
{
    public class AttendanceCheckInRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan CheckIn { get; set; }
    }
}
