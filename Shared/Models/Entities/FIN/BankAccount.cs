using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface BankAccount
    {
        public int BankAccountID { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolder { get; set; }
    }
}
