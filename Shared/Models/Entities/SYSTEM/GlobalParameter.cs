using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.SYSTEM
{
    public interface GlobalParameter
    {
        public string ParaId { get; set; }
        public string ParaName { get; set; }
        public string ParaValues { get; set; }
        public bool ParaStatus { get; set; }
        public string Description { get; set; }
    }
}
