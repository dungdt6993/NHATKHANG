using D69soft.Shared.Models.Entities.FIN;
using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class StockVM : Stock, Department
    {
        public int IsTypeUpdate { get; set; }

        public string StockCode { get; set; }
        public string StockName { get; set; }
        public string StockAddress { get; set; }
        public bool StockActive { get; set; } = true;
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
