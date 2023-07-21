using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface PaymentType
    {
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeName { get; set; }
    }
}
