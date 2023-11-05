using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class RptVM : Rpt, FuncGrp, Module
    {
        //Bien
        public string UserID { get; set; }
        public bool IsChecked { get; set; }

        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
        public int FGNo { get; set; }
        public string FuncGrpID { get; set; }
        public string FuncGrpName { get; set; }
        public string FuncGrpIcon { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
    }
}
