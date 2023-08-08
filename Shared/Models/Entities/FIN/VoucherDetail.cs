using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface VoucherDetail
    {
        public int SeqVD { get; set; }
        public string VNumber { get; set; }
        public string VDDesc { get; set; }
        public float VDQty { get; set; }
        public decimal VDPrice { get; set; }
        public decimal VDDiscountPrice { get; set; }
        public string FromStockCode { get; set; }
        public string ToStockCode { get; set; }
        public string VDNote { get; set; }
        public string InventoryCheck_StockCode { get; set; }
        public float InventoryCheck_Qty { get; set; }
        public float InventoryCheck_ActualQty { get; set; }
    }
}
