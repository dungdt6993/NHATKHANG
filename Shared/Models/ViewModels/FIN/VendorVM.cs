using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class VendorVM : Vendor
    {
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VATCode { get; set; }
        public string VAddress { get; set; }
        public string VNote { get; set; }
        public string Contract_FileScan { get; set; }
        public DateTime? Contract_StartDate { get; set; }
        public DateTime? Contract_EndDate { get; set; }
        public string Contact_Tel { get; set; }
        public bool VendorActive { get; set; }
    }
}
