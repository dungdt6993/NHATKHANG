using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface SalaryTransactionGroup
    {
        public int TrnGroupCode { get; set; }
        public string TrnGroupName { get; set; }
    }
}
