using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Employee
{
    public class UpdateEmployeeByHRRequest
    {
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string NationalId { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string JobTitle { get; set; }
        public bool IsActive { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
