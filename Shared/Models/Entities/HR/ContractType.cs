using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface ContractType
    {
        public string ContractTypeID { get; set; }

        public string ContractTypeName { get; set; }

        public int NumMonth { get; set; }
    }

}
