using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface ISalaryAdjustmentRepository : IGenericRepository<SalaryAdjustment>
    {
        void Update(SalaryAdjustment salaryAdjustment);
    }
}
