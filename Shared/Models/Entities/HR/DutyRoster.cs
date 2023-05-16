using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface DutyRoster
    {
        public DateTime dDate { get; set; }
        public string FirstShiftID { get; set; }
        public string SecondShiftID { get; set; }
    }
}

