using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsClassVM : ItemsClass
    {
        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public int IClsNo { get; set; }
        public bool IClsActive { get; set; }
    }
}
