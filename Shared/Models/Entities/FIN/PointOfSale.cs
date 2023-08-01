using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface PointOfSale
    {
        public string POSCode { get; set; }
        public string POSName { get; set; }
        public string POSAddress { get; set; }
        public string POSTel { get; set; }
    }
}
