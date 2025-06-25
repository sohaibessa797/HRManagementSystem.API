using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Employee
{
    public class EmployeeResponse
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [StringLength(14)]
        public string NationalId { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string JobTitle { get; set; }
        public string? DepartmentName { get; set; }
        public List<string> Roles { get; set; }  

    }
}
