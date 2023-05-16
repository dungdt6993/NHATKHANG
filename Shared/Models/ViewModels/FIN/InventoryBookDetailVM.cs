using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class InventoryBookDetailVM : StockVoucher, Items, ItemsUnit
    {
        //Para
        public float QtyOpen { get; set; }
        public float QtyInput { get; set; }
        public float QtyOutput { get; set; }
        public float QtyEnd { get; set; }
        //Para

        public string VNumber { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public bool IsMultipleInvoice { get; set; }
        public bool VActive { get; set; }
        public string Reference_VNumber { get; set; }
        public string Reference_StockCode { get; set; }
        public string Reference_VSubTypeID { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
