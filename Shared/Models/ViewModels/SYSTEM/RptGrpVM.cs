using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class RptGrpVM : RptGrp, Module
    {
        public int RptGrpID { get; set; }
        public string RptGrpName { get; set; }
        public string ModuleID { get; set; }
        public string ModuleName { get; set; }
        //Bien
        public int IsTypeUpdate { get; set; }
    }
}
