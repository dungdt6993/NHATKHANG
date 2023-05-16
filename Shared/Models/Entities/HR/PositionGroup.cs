using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface PositionGroup
    {
        public int PositionGroupNo { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionGroupName { get; set; }
    }
}
