using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.FIN
{
    interface Bank
    {
        public string SwiftCode { get; set; }
        public string BankShortName { get; set; }
        public string BankFullName { get; set; }
        public string BankNameEng { get; set; }
        public string BankLogo { get; set; }
        public bool BankActive { get; set; }
    }
}
