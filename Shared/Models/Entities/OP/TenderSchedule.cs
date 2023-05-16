using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface TenderSchedule
    {
        public DateTime dDate { get; set; }
        public bool TenderStatus { get; set; }
    }
}
