using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface ItemsGroup
    {
        public string IGrpCode { get; set; }
        public string IGrpName { get; set; }
        public bool IGrpActive { get; set; }
    }
}
