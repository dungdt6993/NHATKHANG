using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.POS
{
    public interface PaymentMode
    {
        public int PNo { get; set; }
        public string PModeCode { get; set; }
        public string PDesc { get; set; }
    }
}
