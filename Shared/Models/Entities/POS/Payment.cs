using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.POS
{
    public interface Payment
    {
        public string PaymentNo { get; set; }
        public DateTime? PDate { get; set; }
        public DateTime? PaymentTime { get; set; }
        public string PNote { get; set; }
        public int isPayment { get; set; }
    }
}
