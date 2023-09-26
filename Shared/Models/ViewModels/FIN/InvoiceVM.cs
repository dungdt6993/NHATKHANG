using D69soft.Shared.Models.Entities.FIN;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class InvoiceVM : Voucher
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
        public int InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public string EserialPerform { get; set; }
    }
}
