using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class GlobalParameterVM : GlobalParameter, Func
    {
        //Para
        public int IsTypeUpdate { get; set; }

        public string ParaId { get; set; }
        public string ParaName { get; set; }
        public string ParaValues { get; set; }
        public bool ParaStatus { get; set; }
        public string Description { get; set; }
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
    }
}
