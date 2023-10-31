using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DayOffVM : Period, Profile, Staff, Department, Position, ALMonth, DOMonth, CLDOMonth, PHMonth, CLPHMonth, Shift
    {
        //Parameter
        public string UserID { get; set; }

        public string dayOffType { get; set; }

        public string ddPHLunar { get; set; }
        public string MMPHLunar { get; set; }
        public string yyyyPHLunar { get; set; }
        public DateTime PHDateLunar { get; set; }

        public int IsTypeUpdate { get; set; }
        //Parameter

        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public string Eserial { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string IDCard { get; set; }
        public DateTimeOffset? DateOfIssue { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTimeOffset? Birthday { get; set; }
        public string PlaceOfBirth { get; set; }
        public int Gender { get; set; }
        public string Resident { get; set; }
        public string Temporarity { get; set; }
        public string Qualification { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string PITTaxCode { get; set; }
        public string VisaNumber { get; set; }
        public DateTimeOffset? VisaExpDate { get; set; }
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
        public DateTimeOffset? JoinDate { get; set; }
        public DateTimeOffset? TerminateDate { get; set; }
        public int Terminated { get; set; }
        public DateTimeOffset? StartDayAL { get; set; }
        public int SalaryByBank { get; set; }
        public string BankAccount { get; set; }
        public int IsPayByMonth { get; set; }
        public int IsPayByDate { get; set; }
        public int IsPayByHour { get; set; }
        public string ReasonTerminate { get; set; }
        public int TimeAttCode { get; set; }
        public string SocialInsNumber { get; set; }
        public string HealthInsNumber { get; set; }
        public string BankCode { get; set; }
        public string EmailCompany { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public bool isLeader { get; set; }
        public string JobDesc { get; set; }
        public int ALCurrentYear { get; set; }
        public int NumMonthAL { get; set; }
        public decimal ALPreMonth { get; set; }
        public decimal ALExperience { get; set; }
        public int ALActive { get; set; }
        public decimal ALAddBalance { get; set; }
        public decimal ALTotal { get; set; }
        public decimal ALTaken { get; set; }
        public decimal ALBalance { get; set; }
        public string ALNoteAddBalance { get; set; }
        public decimal DODefault { get; set; }
        public decimal DOCurrent { get; set; }
        public decimal DOTaken { get; set; }
        public decimal DOBalance { get; set; }
        public decimal CLDOPreMonth { get; set; }
        public decimal CLDOTransfer { get; set; }
        public decimal CLDOAddBalance { get; set; }
        public decimal CLDOTotal { get; set; }
        public decimal CLDOTaken { get; set; }
        public decimal CLDOBalance { get; set; }
        public string CLDONoteAddBalance { get; set; }
        public decimal PHCurrentMonth { get; set; }
        public decimal PHTaken { get; set; }
        public decimal PHWDCL { get; set; }
        public decimal CLPHPreMonth { get; set; }
        public decimal CLPHTransfer { get; set; }
        public decimal CLPHAddBalance { get; set; }
        public decimal CLPHTotal { get; set; }
        public decimal CLPHTaken { get; set; }
        public decimal CLPHBalance { get; set; }
        public string CLPHNoteAddBalance { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTimeOffset? BeginTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }
    }
}
