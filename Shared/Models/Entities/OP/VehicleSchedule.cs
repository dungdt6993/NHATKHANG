using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface VehicleSchedule
    {
        public DateTime dDate { get; set; }
        public bool VehicleStatus { get; set; }
    }
}
