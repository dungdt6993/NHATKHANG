using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class ContractTypeVM : ContractType, ContractTypeGroup
    {
        //Para
        public int IsTypeUpdate { get; set; }

        public string ContractTypeID { get; set; }
        public string ContractTypeName { get; set; }
        public int NumMonth { get; set; }
        public string ContractTypeGroupID { get; set; }
        public string ContractTypeGroupName { get; set; }
    }
}
