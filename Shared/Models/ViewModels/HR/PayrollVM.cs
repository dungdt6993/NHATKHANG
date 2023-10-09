using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class PayrollVM : Period, Profile, Staff, JobHistory, DODefault, Division, Department, Position, Section, ContractType, WorkType, MonthlyIncome, SalaryTransactionCode, SalaryTransactionGroup, SalaryHistory

    {
        //Parameter
        public string UserID { get; set; }
        public decimal TotalSal { get; set; }
        //Parmeter

        public int Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
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
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminateDate { get; set; }
        public int Terminated { get; set; }
        public DateTime? StartDayAL { get; set; }
        public int SalaryByBank { get; set; }
        public string BankAccount { get; set; }
        public int IsPayByMonth { get; set; }
        public int IsPayByDate { get; set; }
        public string ReasonTerminate { get; set; }
        public int TimeAttCode { get; set; }
        public string SocialInsNumber { get; set; }
        public string HealthInsNumber { get; set; }
        public string BankCode { get; set; }
        public string EmailCompany { get; set; }
        public int JobID { get; set; }
        public int SalaryID { get; set; }
        public DateTime? JobStartDate { get; set; }
        public DateTime? StartContractDate { get; set; }
        public DateTime? EndContractDate { get; set; }
        public string WorkTypeID { get; set; }
        public decimal DODefaultNum { get; set; }
        public decimal WTDefault { get; set; }
        public decimal WDDefault { get; set; }
        public decimal PaidDefault { get; set; }
        public decimal BudgetSAT { get; set; }
        public decimal BudgetSUN { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string JobDesc { get; set; }
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string WorkingLocation { get; set; }
        public string TimeAttMachine_SerialLog { get; set; }
        public string ContractTypeID { get; set; }
        public string ContractTypeName { get; set; }
        public int NumMonth { get; set; }
        public string WorkTypeName { get; set; }
        public int SeqMI { get; set; }
        public decimal Amount { get; set; }
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
        public decimal BasicSalary { get; set; }
        public decimal OtherSalary { get; set; }
        public decimal Benefit1 { get; set; }
        public decimal Benefit2 { get; set; }
        public decimal Benefit3 { get; set; }
        public decimal Benefit4 { get; set; }
        public decimal Benefit5 { get; set; }
        public decimal Benefit6 { get; set; }
        public decimal Benefit7 { get; set; }
        public decimal Benefit8 { get; set; }
        public DateTime? BeginSalaryDate { get; set; }
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
    }
}
