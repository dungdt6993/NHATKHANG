using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class SalaryTransactionGroupVM : SalaryTransactionGroup
    {
        public int TrnGroupCode { get; set; }
        public string TrnGroupName { get; set; }
    }
}
