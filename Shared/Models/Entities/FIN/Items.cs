using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    public interface Items
    {
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public string IDetail { get; set; }
        public decimal ICost { get; set; }
        public decimal IPrice { get; set; }
        public decimal IOldPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string VATDefault { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public bool IsSale { get; set; }
    }
}
