using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class FilterVM
    {
        //SYS
        public string UserID { get; set; }
        public string FuncID { get; set; }
        public bool isLeader { get; set; }
        public bool IsOpenFunc { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public DateTimeOffset? dDate { get; set; }

        public int TypeView { get; set; } = 0;

        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

        public string searchText { get; set; }
        public bool IsChecked { get; set; }
        public int ShowEntity { get; set; }

        //FIN
        public int searchActive { get; set; }
        public bool IActive { get; set; }
        public string ICode { get; set; }
        public string StockCode { get; set; }
        public string VTypeID { get; set; }
        public string InvoiceNumber { get; set; }
        public string IClsCode { get; set; }
        public string IGrpCode { get; set; }
        public string ITypeCode { get; set; }
        public string VNumber { get; set; }
        public int Period { get; set; }
        public string RequestStatus { get; set; }
        public bool isHandover { get; set; }

        //HR
        public string DivisionID { get; set; }
        public string searchValues { get; set; }
        public int is2625 { get; set; }
        public string SectionID { get; set; }
        public string DepartmentID { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionID { get; set; }
        public string[] arrPositionID { get; set; } = new string[] { };
        public string Eserial { get; set; }
        public int TypeProfile { get; set; }
        public string[] arrShiftID { get; set; } = new string[] { };
        public string ShiftID { get; set; }
        public string GroupType { get; set; }
        public int DocTypeID { get; set; }
        public int IsTypeSearch { get; set; }
        public string strDataFromExcel { get; set; }
        public int TrnCode { get; set; }
        public int TrnSubCode { get; set; }

        //OP
        public string VehicleCode { get; set; }

        //RPT
        public string ReportName { get; set; }
        public string ModuleID { get; set; }
        public int RptID { get; set; }
        public int RptGrpID { get; set; }
    }
}
