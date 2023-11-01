using System;
using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DepartmentVM : Department, DepartmentGroup, Division, Shift
    {
        //Para
        public bool isActive { get; set; }
        public bool IsChecked { get; set; }
        public int IsTypeUpdate { get; set; }
        public string[] arrPositionID { get; set; } = new string[] { };
        //Para

        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }

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
        public string DepartmentGroupID { get; set; }
        public string DepartmentGroupName { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTimeOffset? BeginTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }
    }
}
