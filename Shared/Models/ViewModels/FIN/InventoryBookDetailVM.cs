﻿using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class InventoryBookDetailVM : Voucher, Items, ItemsUnit
    {
        //Para
        public float QtyOpen { get; set; }
        public float QtyInput { get; set; }
        public float QtyOutput { get; set; }
        public float QtyEnd { get; set; }

        public string VNumber { get; set; }
        public string VReference { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public string VContact { get; set; }
        public bool VActive { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPayment { get; set; }
        public bool IsInvoice { get; set; }
        public string InvoiceSerial { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public string EserialPerform { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public string IDetail { get; set; }
        public decimal ICost { get; set; }
        public decimal IPrice { get; set; }
        public decimal IOldPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string VATDefault { get; set; }
        public string StockDefault { get; set; }
        public bool IActive { get; set; }
        public bool IsSale { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
