using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface LockSalary
    {
        public bool isSalCalc { get; set; }

        public string EserialSalCalc { get; set; }
        public DateTime TimeSalCalc { get; set; }
        public bool isSalLock { get; set; }
        public string EserialSalLock { get; set; }
        public DateTime TimeSalLock { get; set; }
    }
}
