using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class LockDutyRosterVM : Period, Division, Department, LockDutyRoster
    {
        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }
        public string DivisionTaxCode { get; set; }
        public string CodeDivs { get; set; }
        public string DivisionAddress { get; set; }
        public string DivisionTel { get; set; }
        public string DivisionHotline { get; set; }
        public string DivisionEmail { get; set; }
        public string DivisionWebsite { get; set; }
        public string DivisionBankAccount { get; set; }
        public string DivisionBankName { get; set; }
        public string DivisionLogoUrl { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? LockFrom { get; set; }
        public DateTime? LockTo { get; set; }
        public string EserialLock { get; set; }
        public DateTime TimeLock { get; set; }

        //Parameter
        public int IsTypeUpdate { get; set; }

    }
}
