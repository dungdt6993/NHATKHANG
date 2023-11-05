using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class PermissionVM : FuncGrp, Func, SubFunc, Division, Department, Rpt
    {
        public int FGNo { get; set; }
        public string FuncGrpID { get; set; }
        public string FuncGrpName { get; set; }
        public string FuncGrpIcon { get; set; }
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
        public int SubNo { get; set; }
        public string SubFuncID { get; set; }
        public string SubFuncName { get; set; }
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
        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
    }
}
