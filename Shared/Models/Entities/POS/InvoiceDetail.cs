using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.POS
{
    public interface InvoiceDetail
    {
        public int Seq { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string INote { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public decimal Items_DiscountPrice { get; set; }
        public decimal Items_DiscountPercent { get; set; }
        public int isUpdateItem { get; set; }
        public string UpdateItem_By { get; set; }
        public DateTime? UpdateItem_Time { get; set; }
        public string UpdateItem_Note { get; set; }
    }
}
