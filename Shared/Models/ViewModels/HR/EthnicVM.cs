using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class EthnicVM : Ethnic
    {
        public int EthnicID { get; set; }
        public string EthnicName { get; set; }
    }
}
