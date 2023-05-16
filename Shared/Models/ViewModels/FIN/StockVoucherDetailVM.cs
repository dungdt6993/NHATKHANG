using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class StockVoucherDetailVM : Period, StockVoucherDetail, Items, Vendor, ItemsUnit
    {
        //Para
        public string FromStockName { get; set; }
        public string ToStockName { get; set; }
        public string InventoryCheck_StockName { get; set; }

        public int IsUpdateItem { get; set; }
        public int IsUpdateFromStock { get; set; }
        public int IsUpdateToStock { get; set; }
        public int IsUpdateVendor { get; set; }
        public int IsUpdateInventoryCheck_Stock { get; set; }

        //Para

        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int SeqVD { get; set; }
        public string VNumber { get; set; }
        public float Qty { get; set; }
        public decimal Price { get; set; }
        public string FromStockCode { get; set; }
        public string ToStockCode { get; set; }
        public string VendorCode { get; set; }
        public string VDNote { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public string VendorName { get; set; }
        public string VATCode { get; set; }
        public string VAddress { get; set; }
        public string VNote { get; set; }
        public string InventoryCheck_StockCode { get; set; }
        public float InventoryCheck_Qty { get; set; }
        public float InventoryCheck_ActualQty { get; set; }
        public string Request_ICode { get; set; }
        public bool IsReference { get; set; }
        public string Contract_FileScan { get; set; }
        public DateTime? Contract_StartDate { get; set; }
        public DateTime? Contract_EndDate { get; set; }
        public string Contact_Tel { get; set; }
        public bool VendorActive { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
