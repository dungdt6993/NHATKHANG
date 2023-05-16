using System;
using System.Collections.Generic;
using System.Text;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Position
    {
        public string PositionID { get; set; }

        public string PositionName { get; set; }
        public string JobDesc { get; set; }

    }
}
