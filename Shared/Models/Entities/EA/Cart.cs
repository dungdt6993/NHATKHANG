using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.EA
{
    interface Cart
    {
        public int SeqCart { get; set; }
        public string EserialAddToCart { get; set; }
        public float Qty { get; set; }
        public string Note { get; set; }
    }
}
