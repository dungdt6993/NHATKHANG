using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DutyRosterVM : Period, DutyRoster, Profile, Department, PositionGroup, Position, Shift, ShiftType, LockDutyRoster, AttendanceRecord, WorkPlan
    {
        //Parameter
        //DutyRoster
        public string UserID { get; set; }
        public int isPH { get; set; }
        public int dayNum { get; set; }
        public string WorkShift { get; set; }
        public string OldWorkShift { get; set; }

        public Dictionary<string, object> InputAttributes { get; set; }

        public int ckJoined { get; set; }
        public int ckTerminated { get; set; }
        public int isLock { get; set; }
        public bool inputLoading_updateShift { get; set; }
        public string InputID { get; set; }
        public string InputNextID { get; set; }

        //Att
        public float TWD { get; set; }
        public string JobShift { get; set; }
        public float Late { get; set; }
        public float Soon { get; set; }

        //EmplTrf
        public string CruiseStatusCode { get; set; }
        public string CruiseStatus_ColorHEX { get; set; }
        public int GuestNumber { get; set; }
        public bool isCI { get; set; }
        public string DutyRosterNote { get; set; }
        public bool isCheckOFF { get; set; }

        //WorkPlan
        public int IsTypeUpdate { get; set; }

        //Parameter

        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public DateTime dDate { get; set; }
        public string FirstShiftID { get; set; }
        public string SecondShiftID { get; set; }
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
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public bool isLeader { get; set; }
        public string JobDesc { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }

        public DateTime? LockFrom { get; set; }
        public DateTime? LockTo { get; set; }
        public string EserialLock { get; set; }
        public DateTime TimeLock { get; set; }
        public int ARID { get; set; }
        public DateTime? IN1 { get; set; }
        public DateTime? OUT1 { get; set; }
        public DateTime? IN2 { get; set; }
        public DateTime? OUT2 { get; set; }
        public DateTime? IN3 { get; set; }
        public DateTime? OUT3 { get; set; }
        public DateTime? IN4 { get; set; }
        public DateTime? OUT4 { get; set; }
        public int OUT1_isNS { get; set; }
        public int OUT2_isNS { get; set; }
        public int OUT3_isNS { get; set; }
        public int OUT4_isNS { get; set; }
        public string Explain { get; set; }
        public string EserialExplain { get; set; }
        public DateTime? ExplainTime { get; set; }
        public string ExplainHOD { get; set; }
        public string EserialExplainHOD { get; set; }
        public DateTime? ExplainHODTime { get; set; }
        public string ExplainHR { get; set; }
        public string EserialExplainHR { get; set; }
        public DateTime? ExplainHRTime { get; set; }
        public bool isConfirmExplain { get; set; }
        public string EserialConfirmExplain { get; set; }
        public DateTime? TimeConfirmExplain { get; set; }
        public bool isConfirmLateSoon { get; set; }
        public string EserialConfirmLateSoon { get; set; }
        public DateTime? TimeConfirmLateSoon { get; set; }

        public int PositionGroupNo { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionGroupName { get; set; }
        public string ShiftTypeID { get; set; }
        public string ShiftTypeName { get; set; }
        public decimal PercentIncome { get; set; }
        public int isOFF { get; set; }
        public int WorkPlanSeq { get; set; }
        public string WorkPlanName { get; set; }
        public string WorkPlanDesc { get; set; }
        public DateTime? WorkPlanStartDate { get; set; }
        public DateTime? WorkPlanDeadline { get; set; }
        public string WorkPlanNote { get; set; }
        public bool WorkPlanIsDone { get; set; }
        public DateTime? WorkPlanDoneDate { get; set; }
        public string UserCreated { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}

