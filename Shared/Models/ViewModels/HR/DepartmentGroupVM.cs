using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DepartmentGroupVM : DepartmentGroup
    {
        //Para
        public int IsTypeUpdate { get; set; }
        //Para
        public string DepartmentGroupID { get; set; }
        public string DepartmentGroupName { get; set; }
    }
}
