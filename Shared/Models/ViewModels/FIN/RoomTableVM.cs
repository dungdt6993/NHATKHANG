using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class RoomTableVM : RoomTable, Voucher
    {
        public string RoomTableCode { get; set; }
        public string RoomTableName { get; set; }

        public string VNumber { get; set; }
        public string VReference { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public string VContact { get; set; }
        public bool VActive { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPayment { get; set; }
        public bool IsInvoice { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
        public string EserialPerform { get; set; }

        //Bien
        public bool IsOpen { get; set; }
        public DateTimeOffset? TimeCreated { get; set; }
        public string OpenByName { get; set; }
    }
}
