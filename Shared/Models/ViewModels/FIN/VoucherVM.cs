using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class VoucherVM : Division, VType, VSubType, Voucher, Vendor, Customer, Stock, ItemsType, PaymentType, BankAccount
    {
        //Para
        public string UserID { get; set; }
        public int IsTypeUpdate { get; set; }
        public string valueSearchItems { get; set; }

        public string DataFromExcel { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }
        public string DivisionTaxCode { get; set; }
        public string CodeDivs { get; set; }
        public string DivisionAddress { get; set; }
        public string DivisionTel { get; set; }
        public string DivisionHotline { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string VTypeID { get; set; }
        public string VTypeDesc { get; set; }
        public string VCode { get; set; }
        public string VSubTypeID { get; set; }
        public string VSubTypeDesc { get; set; }
        public int AccDebitDefault { get; set; }
        public int AccCreditDefault { get; set; }
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
        public string InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public string EserialPerform { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorTaxCode { get; set; }
        public string VendorAddress { get; set; }
        public string VendorTel { get; set; }
        public string VendorContractFile { get; set; }
        public DateTime? VendorContractStartDate { get; set; }
        public DateTime? VendorContractEndDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockAddress { get; set; }
        public bool StockActive { get; set; }
        public string ITypeCode { get; set; }
        public string ITypeName { get; set; }
        public string ITypeDesc { get; set; }
        public bool IsInventory { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeName { get; set; }
        public int BankAccountID { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolder { get; set; }
    }
}
