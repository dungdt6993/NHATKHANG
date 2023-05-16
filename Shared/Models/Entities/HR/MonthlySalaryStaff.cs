using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    interface MonthlySalaryStaff
    {
        public string ID { get; set; }
        public decimal BasicSalaryActive { get; set; }
        public decimal OtherSalaryActive { get; set; }
        public decimal Benefit1Active { get; set; }
        public decimal Benefit2Active { get; set; }
        public decimal Benefit3Active { get; set; }
        public decimal Benefit4Active { get; set; }
        public decimal Benefit5Active { get; set; }
        public decimal Benefit6Active { get; set; }
        public decimal Benefit7Active { get; set; }
        public decimal Benefit8Active { get; set; }
        public decimal SocialSalaryActive { get; set; }

        public string UrlPaySlipPDF { get; set; }
        public DateTime TimeUpdatePaySlipPDF { get; set; }
        public string SalaryQuestion { get; set; }
        public DateTime TimeSalaryQuestion { get; set; }
        public string SalaryReply { get; set; }
        public string EserialSalaryReply { get; set; }
        public DateTime TimeSalaryReply { get; set; }

        public bool isPrint { get; set; }
        public DateTime TimePrint { get; set; }
        public string EserialPrint { get; set; }
    }
}
