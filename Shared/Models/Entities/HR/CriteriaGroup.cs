using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    interface CriteriaGroup
    {
        public int CriteriaGroupID { get; set; }
        public string CriteriaGroupName { get; set; }
        public float Portion { get; set; }
    }
}
