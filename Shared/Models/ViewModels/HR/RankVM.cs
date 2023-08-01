using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class RankVM : Rank, Department, Position, Profile, Period, DepartmentGroup
    {
        //Para
        public string FullName { get; set; }
        public string Appraiser_FullName { get; set; }
        public string DirectManager_FullName { get; set; }
        public string ControlDept_FullName { get; set; }
        public string Approve_FullName { get; set; }

        public string Appraiser_PositionName { get; set; }
        public string DirectManager_PositionName { get; set; }
        public string ControlDept_PositionName { get; set; }
        public string Approve_PositionName { get; set; }
        //Para

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
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string JobDesc { get; set; }
        public string Eserial { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string IDCard { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTime? Birthday { get; set; }
        public string PlaceOfBirth { get; set; }
        public int Gender { get; set; }
        public string Resident { get; set; }
        public string Temporarity { get; set; }
        public string Qualification { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PITTaxCode { get; set; }
        public string VisaNumber { get; set; }
        public DateTime? VisaExpDate { get; set; }
        public string Image { get; set; }
        public string Hometown { get; set; }
        public string UrlAvatar { get; set; }
        public string User_Password { get; set; }
        public string User_PassReset { get; set; }
        public int User_isChangePass { get; set; }

        public string TaxDept { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Rela { get; set; }
        public string Contact_Tel { get; set; }
        public string Contact_Address { get; set; }
        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string DepartmentGroupID { get; set; }
        public string DepartmentGroupName { get; set; }
    }
}
