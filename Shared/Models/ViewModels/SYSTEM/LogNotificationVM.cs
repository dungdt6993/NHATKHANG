using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class LogNotificationVM : Profile, UserSysLog, UserSysLogNotifi, FuncGrp, Func
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

        public string TaxDept { get; set; }
        public string Contact_Name { get; set; }
        public string Contact_Rela { get; set; }
        public string Contact_Tel { get; set; }
        public string Contact_Address { get; set; }
        public int USLSeq { get; set; }
        public string UserSysLog { get; set; }
        public DateTime TimeUserSysLog { get; set; }
        public string DescUserSysLog { get; set; }
        public string UrlFuncLog { get; set; }
        public string FuncPermisID { get; set; }
        public int isShowNotifi { get; set; }
        public string KeySearch { get; set; }
        public int USLNSeq { get; set; }
        public string UserNotifi { get; set; }
        public int isSeen { get; set; }
        public DateTime? TimeSeen { get; set; }
        public int isNotifi { get; set; }
        public int isNotifiMention { get; set; }
        public int FGNo { get; set; }
        public string FuncGrpID { get; set; }
        public string FuncGrpName { get; set; }
        public string FuncGrpIcon { get; set; }
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
    }
}
