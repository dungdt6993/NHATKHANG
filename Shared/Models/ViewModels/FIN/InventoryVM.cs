﻿using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class InventoryVM : VoucherDetail, Items, ItemsUnit, Stock
    {
        //Para
        public float QtyOpen { get; set; }
        public float QtyInput { get; set; }
        public float QtyOutput { get; set; }
        public float QtyEnd { get; set; }

        public int SeqVD { get; set; }
        public string VNumber { get; set; }
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
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockAddress { get; set; }
        public bool StockActive { get; set; }
        //Para


    }
}
