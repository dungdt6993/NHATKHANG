using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class ShiftVM : Shift, ShiftType
    {
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }

        public string ShiftTypeID { get; set; }
        public string ShiftTypeName { get; set; }
        public decimal PercentIncome { get; set; }
        public int isOFF { get; set; }

        //Bien
        public bool isActive { get; set; }
        public int IsTypeUpdate { get; set; }


    }
}
