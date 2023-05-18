using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities.OP
{
    interface Tender
    {
        public string TenderCode { get; set; }
        public string TenderName { get; set; }
        public bool TenderActive { get; set; }
    }
}
