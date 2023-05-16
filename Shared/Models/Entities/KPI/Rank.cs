using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.KPI
{
    interface Rank
    {
        public float TotalGrandStaffScore { get; set; }
        public float TotalGrandFinalScore { get; set; }
        public string StaffRanking { get; set; }
        public string FinalRanking { get; set; }
        public string SalaryFactor { get; set; }
        public string Appraiser_Eserial { get; set; }
        public string Appraiser_DepartmentID { get; set; }
        public string Appraiser_PositionID { get; set; }
        public string DirectManager_Eserial { get; set; }
        public string DirectManager_DepartmentID { get; set; }
        public string DirectManager_PositionID { get; set; }
        public string ControlDept_Eserial { get; set; }
        public string ControlDept_DepartmentID { get; set; }
        public string ControlDept_PositionID { get; set; }
        public string Approve_Eserial { get; set; }
        public string Approve_DepartmentID { get; set; }
        public string Approve_PositionID { get; set; }
        public bool isSendKPI { get; set; }
        public bool isSendAppraiser { get; set; }
        public bool isSendDirectManager { get; set; }
        public bool isSendControlDept { get; set; }
        public bool isSendApprove { get; set; }
        public DateTime TimeSendKPI { get; set; }
        public DateTime TimeSendAppraiser { get; set; }
        public DateTime TimeSendDirectManager { get; set; }
        public DateTime TimeSendControlDept { get; set; }
        public DateTime TimeSendApprove { get; set; }
        public string StaffNote { get; set; }
        public string ManagerNote { get; set; }
    }
}
