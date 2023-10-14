using D69soft.Shared.Models.Entities.HR;
using System;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class UserVM : Profile, Staff, JobHistory, Division, Department, Position
    {
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
        public string FamilyRegisterCode { get; set; }
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
        public int IsPayByHour { get; set; }
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
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }
        public string DivisionTaxCode { get; set; }
        public string CodeDivs { get; set; }
        public string DivisionAddress { get; set; }
        public string DivisionTel { get; set; }
        public string DivisionHotline { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string JobDesc { get; set; }
    }
}