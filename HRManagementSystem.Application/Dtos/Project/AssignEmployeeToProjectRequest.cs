using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Project
{
    public class AssignEmployeeToProjectRequest
    {
        public List<Guid> EmployeeIds { get; set; } = new();
        public Guid ProjectId { get; set; }
    }
}
