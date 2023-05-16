using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface Cruise
    {
        public string CruiseCode { get; set; }
        public string CruiseName { get; set; }
        public bool CruiseActive { get; set; }
    }
}
