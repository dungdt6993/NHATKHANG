using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface ShiftType
    {
        public string ShiftTypeID { get; set; }
        public string ShiftTypeName { get; set; }
        public decimal PercentIncome { get; set; }
        public int isOFF { get; set; }
    }
}
