using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface QuantitativeItems
    {
        public string QI_ICode { get; set; }
        public double QI_UnitRatio { get; set; }
    }
}
