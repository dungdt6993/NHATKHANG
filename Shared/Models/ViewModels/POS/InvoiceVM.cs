using D69soft.Shared.Models.Entities.CRM;
using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.POS;

namespace D69soft.Shared.Models.ViewModels.POS
{
    public class InvoiceVM : Invoice, InvoiceDetail, PointOfSale, RoomTableArea, RoomTable, ItemsClass, Items, Customer
    {
        //Para
        public decimal IAmount { get; set; }

        public float sumQty { get; set; }
        public decimal sumAmount { get; set; }
        public decimal sumAmountPay { get; set; }
        public int IsClickChangeRoomTable { get; set; }

        public string Reference_VNumber { get; set; }
        public bool Reference_VActive { get; set; }

        //Para


        public string CheckNo { get; set; }
        public DateTime? IDate { get; set; }
        public int isOpen { get; set; }
        public DateTime? OpenTime { get; set; }
        public string OpenBy { get; set; }
        public int isClose { get; set; }
        public DateTime? CloseTime { get; set; }
        public string CloseBy { get; set; }
        public decimal Invoice_DiscountPrice { get; set; }
        public decimal Invoice_DiscountPercent { get; set; }
        public decimal Invoice_TaxPercent { get; set; }
        public decimal Invoice_TotalPaid { get; set; }
        public bool INVActive { get; set; }
        public int Seq { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string INote { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public decimal Items_DiscountPrice { get; set; }
        public decimal Items_DiscountPercent { get; set; }
        public int isUpdateItem { get; set; }
        public string UpdateItem_By { get; set; }
        public DateTime? UpdateItem_Time { get; set; }
        public string UpdateItem_Note { get; set; }
        public string RoomTableAreaID { get; set; }
        public string RoomTableAreaName { get; set; }
        public string RoomTableID { get; set; }
        public string RoomTableName { get; set; }
        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public string IClsDesc { get; set; }
        public bool IClsActive { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }
        public string POSCode { get; set; }
        public string POSName { get; set; }
        public string POSAddress { get; set; }
        public string POSTel { get; set; }
    }
}
