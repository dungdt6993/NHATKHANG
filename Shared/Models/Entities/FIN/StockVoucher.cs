using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface StockVoucher
    {
        public string VNumber { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public bool IsMultipleInvoice { get; set; }
        public bool VActive { get; set; }

        public string Reference_VNumber { get; set; }
        public string Reference_StockCode { get; set; }
        public string Reference_VSubTypeID { get; set; }
    }
}
