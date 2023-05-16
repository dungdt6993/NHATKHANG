using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface SalaryTransactionCode
    {
        public int SalTrnID { get; set; }
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
