using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsUnitVM : ItemsUnit
    {
        //Para
        public int IsTypeUpdate { get; set; }
        //Para

        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
