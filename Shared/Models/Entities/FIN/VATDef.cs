using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface VATDef
    {
        public string VATCode { get; set; }
        public decimal VATRate { get; set; }
        public string VATName { get; set; }
    }
}
