using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DepartmentVM : Department, DepartmentGroup, Division
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
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string DepartmentGroupID { get; set; }
        public string DepartmentGroupName { get; set; }
    }
}
