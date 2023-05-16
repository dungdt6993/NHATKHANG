using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Text;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class UserSysLogVM : UserSysLog
    {
        public int USLSeq { get; set; }
        public string UserSysLog { get; set; }
        public DateTime TimeUserSysLog { get; set; }
        public string DescUserSysLog { get; set; }
        public string UrlFuncLog { get; set; }
        public string FuncPermisID { get; set; }
        public int isShowNotifi { get; set; }
        public string KeySearch { get; set; }
    }
}
