using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class ProfileRelationshipVM : ProfileRelationship, Relationship, Profile
    {
        //Para
        public int IsTypeUpdate { get; set; }

        public int SeqPrRela { get; set; }
        public string Rela_FullName { get; set; }
        public DateTime? Rela_Birthday { get; set; }
        public string Rela_ValidTo { get; set; }
        public string Rela_TaxCode { get; set; }
        public bool isEmployeeTax { get; set; }
        public bool isActive { get; set; }
        public string Rela_Note { get; set; }
        public int RelationshipID { get; set; }
        public string RelationshipName { get; set; }
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
    }
}
