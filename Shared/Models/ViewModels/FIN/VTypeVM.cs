using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.SYSTEM;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class VTypeVM : VType, Func
    {
        public string VTypeID { get; set; }
        public string VTypeDesc { get; set; }
        public string VCode { get; set; }
        public int FNo { get; set; }
        public string FuncID { get; set; }
        public string FuncName { get; set; }
        public string FuncURL { get; set; }
        public bool isActive { get; set; }
    }
}
