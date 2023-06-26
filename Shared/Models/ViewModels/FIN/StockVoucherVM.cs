using D69soft.Shared.Models.Entities.CRM;
using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class StockVoucherVM : Division, VType, VSubType, StockVoucher, Vendor, Stock, Customer
    {
        //Para
        public string UserID { get; set; }
        public int IsTypeUpdate { get; set; }

        public string FullNameEC { get; set; }
        public string FullNameEU { get; set; }
        public DateTime? TimeCreated { get; set; }
        public DateTime? TimeUpdate { get; set; }

        public string Reference_VSubTypeName { get; set; }

        public string DataFromExcel { get; set; }

        //Para

        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }

        public string VTypeID { get; set; }
        public string VTypeDesc { get; set; }
        public string VSubTypeID { get; set; }
        public string VSubTypeDesc { get; set; }

        public string VNumber { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public bool IsMultipleInvoice { get; set; }
        public bool VActive { get; set; }
        public string Reference_VNumber { get; set; }
        public string Reference_StockCode { get; set; }
        public string Reference_VSubTypeID { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorTaxCode { get; set; }
        public string VendorAddress { get; set; }
        public string VendorTel { get; set; }
        public bool VendorActive { get; set; }
        public string VendorContractFile { get; set; }
        public DateTime? VendorContractStartDate { get; set; }
        public DateTime? VendorContractEndDate { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockAddress { get; set; }
        public bool StockActive { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }
    }
}
