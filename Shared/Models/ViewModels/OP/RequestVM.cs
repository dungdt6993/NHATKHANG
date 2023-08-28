using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class RequestVM : Division, Request, Items, ItemsUnit
    {
        //Para
        public string Request_FullName { get; set; }
        public string DirectManager_FullName { get; set; }
        public string ControlDept_FullName { get; set; }
        public string Approve_FullName { get; set; }
        public string Request_DepartmentName { get; set; }
        public bool IsCheckAllHandover { get; set; }
        public bool VActive { get; set; }
        public bool IsUpdateRDNote { get; set; }
        public string DeptRequest { get; set; }

        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string RequestCode { get; set; }
        public string EserialRequest { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string ReasonOfRequest { get; set; }
        public bool isSendDirectManager { get; set; }
        public string DirectManager_Eserial { get; set; }
        public DateTime TimeSendDirectManager { get; set; }
        public bool isSendControlDept { get; set; }
        public string ControlDept_Eserial { get; set; }
        public DateTime TimeSendControlDept { get; set; }
        public bool isSendApprove { get; set; }
        public string Approve_Eserial { get; set; }
        public DateTime TimeSendApprove { get; set; }
        public int SeqRequestDetail { get; set; }
        public float Qty { get; set; }
        public float QtyApproved { get; set; }
        public float QtyHandover { get; set; }
        public string INote { get; set; }
        public string RDNote { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal ICost { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string VATDefault { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public bool IsSale { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
