using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.POS
{
    public class FilterPosVM : PointOfSale, RoomTableArea, RoomTable, Invoice, Items, ItemsGroup, Division
    {
        //Para
        public string UserID { get; set; }

        public string searchValues { get; set; }

        public int searchActive { get; set; }

        public string ReportName { get; set; } = string.Empty;

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        //Para

        public string POSCode { get; set; }
        public string POSName { get; set; }
        public string POSAddress { get; set; }
        public string POSTel { get; set; }
        public string RoomTableAreaID { get; set; } = string.Empty;
        public string RoomTableAreaName { get; set; }
        public string RoomTableID { get; set; }
        public string RoomTableName { get; set; }
        public string ICode { get; set; } = string.Empty;
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
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
        public string IGrpCode { get; set; } = string.Empty;
        public string IGrpName { get; set; }
        public bool IGrpActive { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
    }
}
