using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class MonthlyIncomeTrnOtherVM : MonthlyIncomeTrnOther, Period, Profile, SalaryTransactionCode, SalaryTransactionGroup, EmployeeTransaction
    {
        //Parameter
        public string UserID { get; set; }
        public bool IsChecked { get; set; }
        public int IsTypeUpdate { get; set; }
        public string FirstNameAuthor { get; set; }

        public string strSeqMITrnOther { get; set; }
        //Parameter

        public int SeqMITrnOther { get; set; }
        public string Author { get; set; }
        public DateTime DateUpdate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public string Note { get; set; }
        public bool isPostFromExcel { get; set; }
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
        public int TrnCode { get; set; }
        public int TrnSubCode { get; set; }
        public string TrnName { get; set; }
        public bool isPIT { get; set; }
        public int RatePIT { get; set; }
        public int Rate { get; set; }
        public bool isPaySlip { get; set; }
        public decimal InsPercent { get; set; }
        public int TrnGroupCode { get; set; }
        public string TrnGroupName { get; set; }
        public int SalTrnID { get; set; }
    }
}
