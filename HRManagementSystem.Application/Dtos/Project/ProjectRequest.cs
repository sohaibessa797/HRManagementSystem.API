using HRManagementSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.Project
{
    public class ProjectRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        [Range(0,4)]
        public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;

    }
}
