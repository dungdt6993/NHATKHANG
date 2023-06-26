using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface Vendor
    {
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorTaxCode { get; set; }
        public string VendorAddress { get; set; }
        public string VendorTel { get; set; }
        public string VendorContractFile { get; set; }
        public DateTime? VendorContractStartDate { get; set; }
        public DateTime? VendorContractEndDate { get; set; }
    }
}
