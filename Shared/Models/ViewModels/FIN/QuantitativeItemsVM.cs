using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class QuantitativeItemsVM : QuantitativeItems, Items, ItemsUnit
    {
        //Para
        public string QI_IName { get; set; }
        public string QI_IUnitName { get; set; }
        //Para
        public string QI_ICode { get; set; }
        public double QI_UnitRatio { get; set; }
        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal ICost { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
    }
}
