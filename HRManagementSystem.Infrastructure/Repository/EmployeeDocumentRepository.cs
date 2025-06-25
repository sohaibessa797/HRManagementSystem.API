using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure.Repository
{
    public class EmployeeDocumentRepository : GenericRepository<EmployeeDocument>, IEmployeeDocumentRepository
    {
        public readonly ApplicationDbContext _context;
        public EmployeeDocumentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(EmployeeDocument employeeDocument)
        {
            var existingDocument = _context.EmployeeDocuments.Find(employeeDocument.Id);
            if (existingDocument != null)
            {
                existingDocument.EmployeeId = employeeDocument.EmployeeId;
                existingDocument.FileName = employeeDocument.FileName;
                existingDocument.FilePath = employeeDocument.FilePath;
                existingDocument.Description = employeeDocument.Description;
                existingDocument.CreatedAt = existingDocument.CreatedAt; // Keep the original creation date
            }
            else
            {
                throw new ArgumentException("Employee document not found.");
            }
        }
    }
}
