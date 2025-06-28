using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Employee
{
    public class UpdateEmployeeByHRRequest
    {
        public string? ProfilePictureUrl { get; set; }
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public string JobTitle { get; set; }
        public bool IsActive { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
