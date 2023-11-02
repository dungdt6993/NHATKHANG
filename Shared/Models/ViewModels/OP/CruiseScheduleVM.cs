using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.OP;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class CruiseScheduleVM : Cruise, CruiseStatus, CruiseSchedule, Stock
    {
        //Para
        public decimal sumAmount { get; set; }

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
        public string CruiseScheduleNote { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockAddress { get; set; }
        public bool StockActive { get; set; }
    }
}
