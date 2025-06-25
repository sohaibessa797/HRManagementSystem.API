using HRManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public decimal? Salary { get; set; } 

        [StringLength(14)]
        public string? NationalId { get; set; } 

        [StringLength(250)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; } 

        public bool IsActive { get; set; } = true;

        public Guid? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
        public ICollection<PerformanceReview> PerformanceReviews { get; set; } = new List<PerformanceReview>();
        public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }
}
