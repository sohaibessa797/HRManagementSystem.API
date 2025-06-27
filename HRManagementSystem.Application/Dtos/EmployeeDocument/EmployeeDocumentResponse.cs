using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Dtos.EmployeeDocument
{
    public class EmployeeDocumentResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
