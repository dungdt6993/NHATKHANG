using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface ProfileRelationship
    {
        public int SeqPrRela { get; set; }
        public string Rela_FullName { get; set; }
        public DateTimeOffset? Rela_Birthday { get; set; }
        public string Rela_ValidTo { get; set; }
        public string Rela_TaxCode { get; set; }
        public bool isEmployeeTax { get; set; }
        public bool isActive { get; set; }
        public string Rela_Note { get; set; }
    }
}
