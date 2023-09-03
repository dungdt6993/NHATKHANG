using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class BankAccountVM : BankAccount, Bank
    {
        public int IsTypeUpdate { get; set; }

        public string SwiftCode { get; set; }
        public string BankShortName { get; set; }
        public string BankFullName { get; set; }
        public string BankNameEng { get; set; }
        public string BankLogo { get; set; }
        public bool BankActive { get; set; }
        public int BankAccountID { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolder { get; set; }
    }
}
