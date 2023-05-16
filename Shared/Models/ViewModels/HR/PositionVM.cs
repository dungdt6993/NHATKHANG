using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class PositionVM : Position, PositionGroup
    {
        public string PositionID { get; set; }
        public string PositionName { get; set; }
        public string JobDesc { get; set; }

        //Bien
        public bool isActive { get; set; }
        public int IsTypeUpdate { get; set; }

        public int PositionGroupNo { get; set; }
        public string PositionGroupID { get; set; }
        public string PositionGroupName { get; set; }
    }
}
