using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Notification
{
    public class NotificationRequest
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        [StringLength(250)]
        public string Message { get; set; }
    }
}
