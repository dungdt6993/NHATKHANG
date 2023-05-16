using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface PHMonth
    {
        public decimal PHCurrentMonth { get; set; }
        public decimal PHTaken { get; set; }
        public decimal PHWDCL { get; set; }
    }
}
