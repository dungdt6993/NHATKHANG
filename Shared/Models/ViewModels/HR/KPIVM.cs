using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class KPIVM : Management, CriteriaGroup, Mission, Description, Period, Profile
    {
        public int KPI_ID { get; set; }
        public int KPINo { get; set; }
        public int MyProperty { get; set; }
        public float StaffScore { get; set; }
        public float FinalScore { get; set; }
        public bool isTarget { get; set; }
        public bool isLateSoon { get; set; }
        public string ActualDescription { get; set; }
        public string ManagerComment { get; set; }
        public string TargetCurrent { get; set; }
        public int CriteriaGroupID { get; set; }
        public string CriteriaGroupName { get; set; }
        public float Portion { get; set; }
        public int MissionID { get; set; }
        public string MissionName { get; set; }
        public int DescriptionID { get; set; }
        public string DescriptionName { get; set; }
        public float Score { get; set; }
        public int No { get; set; }
        public int MinScore { get; set; }
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

        public string TaxDept { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Rela { get; set; }
        public string Contact_Tel { get; set; }
        public string Contact_Address { get; set; }
    }
}
