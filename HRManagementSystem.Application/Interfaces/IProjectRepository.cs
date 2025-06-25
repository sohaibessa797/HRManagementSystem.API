using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        void Update(Project project);
    }
}
