using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.LeaveRequest
{
    public class LeaveRequestRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
