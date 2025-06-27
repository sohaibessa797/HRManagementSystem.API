using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.LeaveRequest
{
    public class LeaveRequestRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        [Range(0,3)]
        public LeaveType LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
