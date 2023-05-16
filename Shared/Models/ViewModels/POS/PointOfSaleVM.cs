using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.POS
{
    public class PointOfSaleVM : PointOfSale, Division, Stock
    {
        public string POSCode { get; set; }
        public string POSName { get; set; }
        public string POSAddress { get; set; }
        public string POSTel { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string CodeDivs { get; set; }
        public string DivsAddress { get; set; }
        public string DivsTel { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string SAddress { get; set; }
        public bool SActive { get; set; }
    }
}
