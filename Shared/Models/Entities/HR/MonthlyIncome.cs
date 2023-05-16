using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface MonthlyIncome
    {
        public int SeqMI { get; set; }
        public decimal Amount { get; set; }
    }
}
