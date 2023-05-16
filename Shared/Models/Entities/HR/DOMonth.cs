using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface DOMonth
    {
        public decimal DODefault { get; set; }
        public decimal DOCurrent { get; set; }
        public decimal DOTaken { get; set; }
        public decimal DOBalance { get; set; }
    }
}
