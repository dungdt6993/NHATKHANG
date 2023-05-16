using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface DODefault
    {
        public string WorkTypeID { get; set; }
        public decimal DODefaultNum { get; set; }
        public decimal WTDefault { get; set; }
        public decimal WDDefault { get; set; }
        public decimal PaidDefault { get; set; }
        public decimal BudgetSAT { get; set; }
        public decimal BudgetSUN { get; set; }
    }
}
