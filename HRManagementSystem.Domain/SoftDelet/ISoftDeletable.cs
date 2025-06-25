using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SoftDelet
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
