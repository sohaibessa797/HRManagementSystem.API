using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Employee
{
    public class EmployeeRequest
    {
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? JobTitle { get; set; }
        public string? NationalId { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
