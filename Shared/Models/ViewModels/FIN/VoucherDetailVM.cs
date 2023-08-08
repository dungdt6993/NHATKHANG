using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class VoucherDetailVM : Period, VoucherDetail, Items, ItemsUnit, VATDef
    {
        //Para
        public string FromStockName { get; set; }
        public string ToStockName { get; set; }
        public string InventoryCheck_StockName { get; set; }
        public decimal VDAmount { get; set; }
        public decimal VDDiscountPercent { get; set; }

        //Rpt
        public DateTime VDate { get; set; }
        public string VNumberInput { get; set; }
        public string VNumberOutput { get; set; }
        public decimal VDPriceOpen { get; set; }
        public decimal VDPriceInput { get; set; }
        public decimal VDPriceOutput { get; set; }
        public decimal VDPriceEnd { get; set; }

        public int IsUpdateItem { get; set; }
        public int IsUpdateFromStock { get; set; }
        public int IsUpdateToStock { get; set; }
        public int IsUpdateVendor { get; set; }
        public int IsUpdateInventoryCheck_Stock { get; set; }
        public int IsUpdateVAT { get; set; }
        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
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
        public string VATCode { get; set; }
        public float VATRate { get; set; }
        public string VATName { get; set; }

    }
}
