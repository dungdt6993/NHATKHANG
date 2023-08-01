using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    interface Description
    {
        public int DescriptionID { get; set; }
        public string DescriptionName { get; set; }
        public float Score { get; set; }
        public int No { get; set; }
        public int MinScore { get; set; }
    }
}
