using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class ItemsVM : Items, ItemsClass, ItemsGroup, ItemsUnit, ItemsType, VATDef
    {
        //Para
        public int IsTypeUpdate { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDelFileUpload { get; set; }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileType { get; set; }
        public decimal VDPrice { get; set; }

        public string ICode { get; set; }
        public string IBarCode { get; set; }
        public string IName { get; set; }
        public string IInvoiceName { get; set; }
        public string IDetail { get; set; }
        public decimal ICost { get; set; }
        public decimal IPrice { get; set; }
        public decimal IOldPrice { get; set; }
        public string IURLPicture1 { get; set; }
        public string VATDefault { get; set; }
        public string StockDefault { get; set; }
        public bool IActive { get; set; }
        public bool IsSale { get; set; }
        public string IClsCode { get; set; }
        public string IClsName { get; set; }
        public int IClsNo { get; set; }
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
        public string VATCode { get; set; }
        public decimal VATRate { get; set; }
        public string VATName { get; set; }
    }
}
