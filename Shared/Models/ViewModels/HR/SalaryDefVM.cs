using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Text;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class SalaryDefVM : SalaryDef, SalaryTransactionCode
    {
        //Parameter
        public bool isUpdate { get; set; }
        public bool isSave { get; set; }
        public bool isNoShowBtnUpdate { get; set; }
        //Parameter

        public int SalTrnID { get; set; }
        public string SalaryType { get; set; }
        public string SalaryTypeName { get; set; }
        public bool isSIns { get; set; }
        public bool isCalcByShift { get; set; }
        public int TrnCode { get; set; }
        public int TrnSubCode { get; set; }
        public string TrnName { get; set; }
        public int RatePIT { get; set; }
        public int Rate { get; set; }
        public int TrnGroupCode { get; set; }
        public bool isPaySlip { get; set; }
        public decimal InsPercent { get; set; }
        public bool isPIT { get; set; }
    }
}
