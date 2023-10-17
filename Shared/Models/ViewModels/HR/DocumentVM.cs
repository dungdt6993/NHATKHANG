using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class DocumentVM : Document, DocumentType, Division, Department, Profile
    {
        //parameter
        public bool IsExpDoc { get; set; }
        public int IsTypeUpdate { get; set; }

        //Upload file scan
        public bool IsDelFileScan { get; set; }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        //parameter

        public int DocID { get; set; }
        public string DocName { get; set; }
        public string TextNumber { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? ExpDate { get; set; }
        public string DocNote { get; set; }
        public string FileScan { get; set; }
        public string isActive { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public int NumExpDate { get; set; }
        public string GroupType { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Eserial { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string IDCard { get; set; }
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
    }
}
