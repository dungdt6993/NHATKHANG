using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class AgreementTextVM : LogPrintAgreementText, Rpt, AdjustProfile, Profile, Department, Position
    {
        public int Seq { get; set; }
        public DateTime dDate { get; set; }
        public int isPrint { get; set; }
        public string EserialPrint { get; set; }
        public DateTime? TimePrint { get; set; }
        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
        public string AdjustProfileID { get; set; }
        public string AdjustProfileName { get; set; }
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

        //Parameter
        public bool IsChecked { get; set; }
        public string FullNamePrint { get; set; }
    }
}
