using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class PayslipVM : MonthlySalaryStaff, Period, Profile, Department, Position, LockSalary
    {
        //Para
        public bool IsChecked { get; set; }
        public string UserID { get; set; }

        public string FullNameSalaryReply { get; set; }
        public string UrlAvatarSalaryReply { get; set; }

        public int TypeUpdateSalaryQuestion { get; set; }
        //Para

        public string ID { get; set; }
        public decimal BasicSalaryActive { get; set; }
        public decimal OtherSalaryActive { get; set; }
        public decimal Benefit1Active { get; set; }
        public decimal Benefit2Active { get; set; }
        public decimal Benefit3Active { get; set; }
        public decimal Benefit4Active { get; set; }
        public decimal Benefit5Active { get; set; }
        public decimal Benefit6Active { get; set; }
        public decimal Benefit7Active { get; set; }
        public decimal Benefit8Active { get; set; }
        public decimal SocialSalaryActive { get; set; }
        public string UrlPaySlipPDF { get; set; }
        public DateTime TimeUpdatePaySlipPDF { get; set; }
        public string SalaryQuestion { get; set; }
        public DateTime TimeSalaryQuestion { get; set; }
        public string SalaryReply { get; set; }
        public string EserialSalaryReply { get; set; }
        public DateTime TimeSalaryReply { get; set; }
        public bool isPrint { get; set; }
        public DateTime TimePrint { get; set; }
        public string EserialPrint { get; set; }
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
        public DateTime? VisaExpDate { get; set; }
        public string Image { get; set; }
        public string Hometown { get; set; }
        public string UrlAvatar { get; set; }
        public string User_Password { get; set; }
        public string User_PassReset { get; set; }
        public int User_isChangePass { get; set; }
        public string FamilyRegisterCode { get; set; }
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
        public bool isSalCalc { get; set; }
        public string EserialSalCalc { get; set; }
        public DateTime TimeSalCalc { get; set; }
        public bool isSalLock { get; set; }
        public string EserialSalLock { get; set; }
        public DateTime TimeSalLock { get; set; }
    }
}
