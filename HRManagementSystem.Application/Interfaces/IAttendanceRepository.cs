using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        void Update(Attendance attendance);
    }
}
