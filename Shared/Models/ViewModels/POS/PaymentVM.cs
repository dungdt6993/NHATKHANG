using D69soft.Shared.Models.Entities.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.POS
{
    public class PaymentVM : Payment, PaymentMode, Invoice
    {
        public string PaymentNo { get; set; }
        public DateTime? PDate { get; set; }
        public DateTime? PaymentTime { get; set; }
        public string PNote { get; set; }
        public int isPayment { get; set; }
        public int PNo { get; set; }
        public string PModeCode { get; set; }
        public string PDesc { get; set; }
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

        //Bien
        public decimal sumAmountPay { get; set; }
        public decimal CustomerAmount { get; set; }

        public decimal ReturnAmount { get; set; }

        public decimal CustomerAmountSuggest { get; set; }

    }
}
