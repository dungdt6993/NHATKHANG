using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.KPI
{
    interface Management
    {
        public int KPI_ID { get; set; }
        public int KPINo { get; set; }
        public int MyProperty { get; set; }
        public float StaffScore { get; set; }
        public float FinalScore { get; set; }
        public bool isTarget { get; set; }
        public bool isLateSoon { get; set; }
        public string ActualDescription { get; set; }
        public string ManagerComment { get; set; }

        public string TargetCurrent { get; set; }
    }
}
