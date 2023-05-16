using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class RptVM : Rpt, RptGrp, Module
    {
        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
        public int RptGrpID { get; set; }
        public string RptGrpName { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }

        //Bien
        public string UserID { get; set; }
        public int IsTypeUpdate { get; set; }
    }
}
