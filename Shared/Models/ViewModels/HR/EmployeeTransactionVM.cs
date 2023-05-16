using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class EmployeeTransactionVM : EmployeeTransaction, SalaryTransactionGroup, SalaryTransactionCode
    {
        //Parameter
        public bool IsChecked { get; set; }
        //Parameter

        public int SalTrnID { get; set; }
        public int TrnGroupCode { get; set; }
        public string TrnGroupName { get; set; }
        public int TrnCode { get; set; }
        public int TrnSubCode { get; set; }
        public string TrnName { get; set; }
        public bool isPIT { get; set; }
        public int RatePIT { get; set; }
        public int Rate { get; set; }
        public bool isPaySlip { get; set; }
        public decimal InsPercent { get; set; }
    }
}
