using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class FilterFinVM : Division, Department, ItemsClass, ItemsGroup, Items, VSubType, StockVoucher, Func, StockType, Stock
    {
        //Parameter
        public string UserID { get; set; }
        public bool IsChecked { get; set; }
        public string selectedICode { get; set; }
        public string searchText { get; set; }
        public int searchActive { get; set; }

        //Request
        public bool isHandover { get; set; }
        public string RequestStatus { get; set; }
        public int ShowEntity { get; set; }

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        //Parameter

        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }

        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public string IClsDesc { get; set; }
        public bool IClsActive { get; set; }
        public string IGrpCode { get; set; }
        public string IGrpName { get; set; }
        public bool IGrpActive { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
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
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
        public string StockTypeCode { get; set; }
        public string StockTypeName { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string SAddress { get; set; }
        public bool SActive { get; set; }
    }
}
