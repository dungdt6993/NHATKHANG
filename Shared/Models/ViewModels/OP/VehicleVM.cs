using D69soft.Shared.Models.Entities.HR;
using Model.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels.OP
{
    public class VehicleVM : Vehicle, Department
    {
        public int IsTypeUpdate { get; set; }

        public string VehicleCode { get; set; }
        public string VehicleName { get; set; }
        public bool VehicleActive { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
