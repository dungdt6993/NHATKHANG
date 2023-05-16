using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class LockSalaryVM : LockSalary, Period
    {
        //Parameter
        public string FullNameSalCalc { get; set; }
        public string FullNameSalLock { get; set; }

        public bool isSalCalc { get; set; }
        public string EserialSalCalc { get; set; }
        public DateTime TimeSalCalc { get; set; }
        public bool StatusSalCalc { get; set; }
        public bool isSalLock { get; set; }
        public string EserialSalLock { get; set; }
        public DateTime TimeSalLock { get; set; }
        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
