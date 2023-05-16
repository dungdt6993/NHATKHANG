using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using D69soft.Shared.Models.Entities.FIN;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class VSubTypeVM : VSubType, VType
    {
        public string VSubTypeID { get; set; }
        public string VSubTypeDesc { get; set; }
        public string VTypeID { get; set; }
        public string VTypeDesc { get; set; }
    }
}
