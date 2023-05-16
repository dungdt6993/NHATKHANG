using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface MonthlyIncomeTrnOther
    {
        public int SeqMITrnOther { get; set; }
        public string Author { get; set; }
        public DateTime DateUpdate { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Qty { get; set; }
        public string Note { get; set; }
        public bool isPostFromExcel { get; set; }
    }
}
