using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    public interface Invoice
    {
        public string CheckNo { get; set; }
        public DateTime? IDate { get; set; }
        public int isOpen { get; set; }
        public DateTime? OpenTime { get; set; }
        public string OpenBy { get; set; }
        public int isClose { get; set; }
        public DateTime? CloseTime { get; set; }
        public string CloseBy { get; set; }
        public decimal Invoice_DiscountPrice { get; set; }
        public decimal Invoice_DiscountPercent { get; set; }
        public decimal Invoice_TaxPercent { get; set; }
        public decimal Invoice_TotalPaid { get; set; }

        public bool INVActive { get; set; }
    }
}
