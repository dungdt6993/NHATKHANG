using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class AccountVM : Account
    {
        public int AccountNo { get; set; }
        public int AccountName { get; set; }
    }
}
