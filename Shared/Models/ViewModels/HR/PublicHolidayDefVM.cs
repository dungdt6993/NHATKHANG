using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class PublicHolidayDefVM : PublicHolidayDef
    {
        //Parameter
        public int IsTypeUpdate { get; set; }

        public int PHDefID { get; set; }
        public string PHName { get; set; }
        public int PHDay { get; set; }
        public int PHMonth { get; set; }
        public int NumDay { get; set; }
        public bool isLunar { get; set; }
    }
}
