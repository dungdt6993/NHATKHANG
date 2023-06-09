using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsVM : Items, ItemsClass, ItemsGroup, ItemsUnit, ItemsType
    {
        //Para
        public int IsTypeUpdate { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDelFileUpload { get; set; }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        //Para

        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public decimal IPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string StockDefault { get; set; }
        public string VendorDefault { get; set; }
        public bool IActive { get; set; }
        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public string IClsDesc { get; set; }
        public bool IClsActive { get; set; }
        public string IGrpCode { get; set; }
        public string IGrpName { get; set; }
        public bool IGrpActive { get; set; }
        public string IUnitCode { get; set; }
        public string IUnitName { get; set; }
        public string ITypeCode { get; set; }
        public string ITypeName { get; set; }
        public string ITypeDesc { get; set; }
        public bool IsInventory { get; set; }
    }
}
