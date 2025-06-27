using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Project
{
    public class EmployeeProjectResponse
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
