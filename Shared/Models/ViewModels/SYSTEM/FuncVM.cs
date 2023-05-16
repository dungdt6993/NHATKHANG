using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Text;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class FuncVM : Module, FuncGrp, Func, SubFunc
    {
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int FGNo { get; set; }
        public string FuncGrpID { get; set; }
        public string FuncGrpName { get; set; }
        public string FuncGrpIcon { get; set; }
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
        public int SubNo { get; set; }
        public string SubFuncID { get; set; }
        public string SubFuncName { get; set; }

        //------------------Biến--------------------//
        public bool IsChecked { get; set; }
    }
}
