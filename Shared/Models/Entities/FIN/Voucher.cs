﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface Voucher
    {
        public string VNumber { get; set; }
        public string VDesc { get; set; }
        public DateTimeOffset? VDate { get; set; }
        public bool VActive { get; set; }
        public bool IsPayment { get; set; }
        public bool IsInvoice { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTimeOffset? InvoiceDate { get; set; }
    }
}