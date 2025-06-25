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
    public class SalaryAdjustmentRepository : GenericRepository<SalaryAdjustment>, ISalaryAdjustmentRepository
    {
        private readonly ApplicationDbContext _context;
        public SalaryAdjustmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(SalaryAdjustment salaryAdjustment)
        {
            var existingAdjustment = _context.SalaryAdjustments.Find(salaryAdjustment.Id);
            if (existingAdjustment != null)
            {
                existingAdjustment.EmployeeId = salaryAdjustment.EmployeeId;
                existingAdjustment.Amount = salaryAdjustment.Amount;
                existingAdjustment.Type = salaryAdjustment.Type;
                existingAdjustment.EffectiveDate = salaryAdjustment.EffectiveDate;
                existingAdjustment.Reason = salaryAdjustment.Reason;
            }
            else
            {
                throw new KeyNotFoundException("Salary adjustment not found.");
            }
        }
    }
}
