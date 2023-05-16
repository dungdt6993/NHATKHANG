using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class WorkTypeVM : WorkType
    {
        //Parameter
        public int IsTypeUpdate { get; set; }

        public string WorkTypeID { get; set; }
        public string WorkTypeName { get; set; }
        public decimal BudgetSATConfig { get; set; }
        public decimal BudgetSUNConfig { get; set; }
        public bool isCalcCLDO { get; set; }

    }
}
