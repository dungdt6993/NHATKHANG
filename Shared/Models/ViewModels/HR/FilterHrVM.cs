using System;
using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.OP;
using D69soft.Shared.Models.Entities.SYSTEM;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class FilterHrVM : Period, Profile, Division, Department, Section, Position, PositionGroup, Rpt, Shift, SalaryTransactionGroup, SalaryTransactionCode, Cruise
    {
        //Parameter
        public string UserID { get; set; }

        public int TypeProfile { get; set; }
        public string selectedEserial { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }

        public DateTimeOffset? dDate { get; set; }

        public string[] arrPositionID { get; set; } = new string[] { };

        public bool IsChecked { get; set; }

        public string strDataFromExcel { get; set; }
        public int isTypeSearch { get; set; }

        public string searchValues { get; set; }
        public string searchEmpl { get; set; }

        public string[] arrShiftID { get; set; } = new string[] { };

        public int typeView { get; set; } = 1;

        //Document
        public string GroupType { get; set; }
        public int DocTypeID { get; set; }

        //Parameter

        public int Period { get; set; }

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
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string WorkingLocation { get; set; }
        public string TimeAttMachine_SerialLog { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string JobDesc { get; set; }
        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }
        public int SalTrnID { get; set; }
        public int TrnCode { get; set; }
        public int TrnSubCode { get; set; }
        public string TrnName { get; set; }
        public bool isPIT { get; set; }
        public int RatePIT { get; set; }
        public int Rate { get; set; }
        public int TrnGroupCode { get; set; }
        public bool isPaySlip { get; set; }
        public decimal InsPercent { get; set; }
        public string TrnGroupName { get; set; }

        public int PositionGroupNo { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionGroupName { get; set; }
        public string CruiseCode { get; set; }
        public string CruiseName { get; set; }
        public bool CruiseActive { get; set; }
    }
}
