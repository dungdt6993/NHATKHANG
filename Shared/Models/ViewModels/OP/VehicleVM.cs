using Model.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModels.OP
{
    public class VehicleVM : Vehicle
    {
        public string VehicleCode { get; set; }
        public string VehicleName { get; set; }
        public bool VehicleActive { get; set; }
    }
}
