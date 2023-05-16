using D69soft.Shared.Models.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class CruiseScheduleVM : Cruise, CruiseStatus, CruiseSchedule
    {
        public string CruiseCode { get; set; }
        public string CruiseName { get; set; }
        public bool CruiseActive { get; set; }
        public string CruiseStatusCode { get; set; }
        public string CruiseStatusName { get; set; }
        public int NumDay { get; set; }
        public string ColorHEX { get; set; }
        public DateTime dDate { get; set; }
        public bool isCI { get; set; }
        public int GuestNumber { get; set; }
        public decimal BudgetFoodCost { get; set; }
        public bool isUpdateFoodCost { get; set; }
        public string ReasonUpdateFoodCost { get; set; }
        public string CruiseScheduleNote { get; set; }
    }
}
