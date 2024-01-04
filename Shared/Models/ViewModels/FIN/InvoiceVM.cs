﻿using D69soft.Shared.Models.Entities.FIN;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class InvoiceVM : Voucher, VoucherDetail, VType, Items, VATDef
    {
        //Para
        public string ObjectName { get; set; }
        public string TaxCode { get; set; }
        public decimal sumVDAmount { get; set; }
        public decimal sumVDDiscountAmount { get; set; }
        public decimal sumVATAmount { get; set; }
        public decimal sumTotalAmount { get; set; }

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
        public string VTypeID { get; set; }
        public string VTypeDesc { get; set; }
        public string VCode { get; set; }
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
        public int SeqVD { get; set; }
        public string VDDesc { get; set; }
        public decimal VDQty { get; set; }
        public decimal VDPrice { get; set; }
        public decimal VDAmount { get; set; }
        public decimal VDDiscountPercent { get; set; }
        public decimal VDDiscountAmount { get; set; }
        public string IDescTax { get; set; }
        public decimal VATAmount { get; set; }
        public string FromStockCode { get; set; }
        public string ToStockCode { get; set; }
        public string VDNote { get; set; }
        public string InventoryCheck_StockCode { get; set; }
        public decimal InventoryCheck_Qty { get; set; }
        public decimal InventoryCheck_ActualQty { get; set; }
        public int DebitAccount { get; set; }
        public int CreditAccount { get; set; }
        public int TaxAccount { get; set; }
        public string VATCode { get; set; }
        public decimal VATRate { get; set; }
        public string VATName { get; set; }
    }
}
