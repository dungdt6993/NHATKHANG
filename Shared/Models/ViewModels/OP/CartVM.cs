using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.OP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class CartVM : Cart, Items, ItemsUnit
    {
        //Para
        public string UserID { get; set; }
        //Para

        public int SeqCart { get; set; }
        public string EserialAddToCart { get; set; }
        public float Qty { get; set; }
        public string Note { get; set; }
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
        public bool IActive { get; set; }
        public bool IsSale { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
