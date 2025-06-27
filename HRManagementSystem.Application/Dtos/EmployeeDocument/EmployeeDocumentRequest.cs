using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.EmployeeDocument
{
    public class EmployeeDocumentRequest
    {
        public Guid EmployeeId { get; set; }
        public string FileName { get; set; }
        public string? Description { get; set; }
    }
}
