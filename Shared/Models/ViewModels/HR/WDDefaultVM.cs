using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class WDDefaultVM : Period, WorkType, DODefault
    {
        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string WorkTypeID { get; set; }
        public string WorkTypeName { get; set; }
        public decimal DODefaultNum { get; set; }
        public decimal WTDefault { get; set; }
        public decimal WDDefault { get; set; }
        public decimal PaidDefault { get; set; }
        public decimal BudgetSAT { get; set; }
        public decimal BudgetSUN { get; set; }
    }
}
