using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class StockTypeVM : StockType
    {
        public string StockTypeCode { get; set; }
        public string StockTypeName { get; set; }
    }
}
