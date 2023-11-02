using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface CruiseSchedule
    {
        public DateTime dDate { get; set; }
        public bool isCI { get; set; }
        public int GuestNumber { get; set; }
        public string CruiseScheduleNote { get; set; }
    }
}
