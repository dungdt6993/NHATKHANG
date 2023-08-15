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
        public decimal VDQty { get; set; }
        public decimal VDPrice { get; set; }
        public decimal VDAmount { get; set; }
        public decimal VDDiscountPercent { get; set; }
        public decimal VDDiscountAmount { get; set; }
        public decimal VATAmount { get; set; }
        public string FromStockCode { get; set; }
        public string ToStockCode { get; set; }
        public string VDNote { get; set; }
        public string InventoryCheck_StockCode { get; set; }
        public decimal InventoryCheck_Qty { get; set; }
        public decimal InventoryCheck_ActualQty { get; set; }
    }
}
