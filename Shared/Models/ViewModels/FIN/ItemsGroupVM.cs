using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsGroupVM : ItemsClass, ItemsGroup
    {
        //Para
        public int IsTypeUpdate { get; set; }
        //Para

        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public string IClsDesc { get; set; }
        public bool IClsActive { get; set; }
        public string IGrpCode { get; set; }
        public string IGrpName { get; set; }
        public bool IGrpActive { get; set; }
    }
}
