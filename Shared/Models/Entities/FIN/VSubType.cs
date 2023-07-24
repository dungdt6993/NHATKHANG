using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface VSubType
    {
        public string VSubTypeID { get; set; }
        public string VSubTypeDesc { get; set; }
        public int AccDebitDefault { get; set; }
        public int AccCreditDefault { get; set; }
    }
}
