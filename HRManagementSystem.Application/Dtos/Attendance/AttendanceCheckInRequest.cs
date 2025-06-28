using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Attendance
{
    public class AttendanceCheckInRequest
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan CheckIn { get; set; }
    }
}
