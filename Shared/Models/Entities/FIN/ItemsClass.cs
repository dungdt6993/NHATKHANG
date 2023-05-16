using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface ItemsClass
    {
        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public string IClsDesc { get; set; }
        public bool IClsActive { get; set; }
    }
}
