using D69soft.Shared.Models.Entities.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.POS
{
    public class PaymentModeVM : PaymentMode
    {
        public int PNo { get; set; }
        public string PModeCode { get; set; }
        public string PDesc { get; set; }
    }
}
