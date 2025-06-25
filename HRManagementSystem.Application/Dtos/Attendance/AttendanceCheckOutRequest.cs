using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Attendance
{
    public class AttendanceCheckOutRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan CheckOut { get; set; }
    }
}
