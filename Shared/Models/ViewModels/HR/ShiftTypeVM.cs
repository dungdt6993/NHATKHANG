using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class ShiftTypeVM : ShiftType
    {
        public string ShiftTypeID { get; set; }
        public string ShiftTypeName { get; set; }
        public decimal PercentIncome { get; set; }
        public int isOFF { get; set; }
    }
}
