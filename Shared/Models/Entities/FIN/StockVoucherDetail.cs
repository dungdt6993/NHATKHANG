using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface StockVoucherDetail
    {
        public int SeqVD { get; set; }
        public string VNumber { get; set; }
        public float Qty { get; set; }
        public decimal Price { get; set; }
        public string FromStockCode { get; set; }
        public string ToStockCode { get; set; }
        public string VendorCode { get; set; }
        public string VDNote { get; set; }
        public string InventoryCheck_StockCode { get; set; }
        public float InventoryCheck_Qty { get; set; }
        public float InventoryCheck_ActualQty { get; set; }
        public string Request_ICode { get; set; }
        public bool IsReference { get; set; }
    }
}
