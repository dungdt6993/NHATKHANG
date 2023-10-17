using System;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class ProfileVM : Profile, Staff, JobHistory, SalaryHistory, Division, Department, DepartmentGroup, Position, PositionGroup, Section, Country, Ethnic, Shift, ContractType, WorkType, PermissionUser, JobSalHistory, AdjustProfile, Rank
    {
        //Para
        public int ckContractExtension { get; set; }
        public int ckJob { get; set; }
        public int ckSal { get; set; }
        public string UserID { get; set; }
        public DateTime Old_StartContractDate { get; set; }
        public DateTime Old_JobStartDate { get; set; }
        public DateTime Old_BeginSalaryDate { get; set; }
        public decimal TotalSalary { get; set; }
        public int IsTypeUpdate { get; set; }
        public DateTime DateJSH { get; set; }

        public string FullName { get; set; }

        //Upload file scan
        public bool IsDelFileUpload { get; set; }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }

        //Appraiser
        public string Appraiser_FullName { get; set; }
        public string DirectManager_FullName { get; set; }
        public string ControlDept_FullName { get; set; }
        public string Approve_FullName { get; set; }

        public string Appraiser_UrlAvatar { get; set; }
        public string DirectManager_UrlAvatar { get; set; }
        public string ControlDept_UrlAvatar { get; set; }
        public string Approve_UrlAvatar { get; set; }

        //Para

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

        public int JobID { get; set; }
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
        public string DivisionEmail { get; set; }
        public string DivisionWebsite { get; set; }
        public string DivisionBankAccount { get; set; }
        public string DivisionBankName { get; set; }
        public string DivisionLogoUrl { get; set; }
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
        public string AgreementText_Signature_Name { get; set; }
        public string AgreementText_Signature_Position { get; set; }
        public string TimeAttMachine_SerialLog { get; set; }

        public int SalaryID { get; set; }
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

        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public int EthnicID { get; set; }
        public string EthnicName { get; set; }

        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }

        public string ContractTypeID { get; set; }
        public string ContractTypeName { get; set; }
        public int NumMonth { get; set; }

        public string WorkTypeID { get; set; }
        public string WorkTypeName { get; set; }

        public int PermisId { get; set; }
        public string Description { get; set; }

        public int isPrintLaborContract { get; set; }
        public string AdjustProfileID { get; set; }
        public string AdjustProfileName { get; set; }
        public int PositionGroupNo { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionGroupName { get; set; }
        public string DepartmentGroupID { get; set; }
        public string DepartmentGroupName { get; set; }
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
