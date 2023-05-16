using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface PublicHolidayDef
    {
        public int PHDefID { get; set; }
        public string PHName { get; set; }
        public int PHDay { get; set; }
        public int PHMonth { get; set; }
        public int NumDay { get; set; }
        public bool isLunar { get; set; }
    }
}
