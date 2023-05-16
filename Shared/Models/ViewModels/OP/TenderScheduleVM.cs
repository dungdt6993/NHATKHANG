using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class TenderScheduleVM : Tender, TenderSchedule, Shift
    {
        public string TenderCode { get; set; }
        public string TenderName { get; set; }
        public bool TenderActive { get; set; }
        public DateTime dDate { get; set; }
        public bool TenderStatus { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }
    }
}
