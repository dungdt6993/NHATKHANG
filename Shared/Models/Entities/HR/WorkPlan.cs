using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    interface WorkPlan
    {
        public int WorkPlanSeq { get; set; }
        public string WorkPlanName { get; set; }
        public string WorkPlanDesc { get; set; }
        public DateTimeOffset? WorkPlanStartDate { get; set; }
        public DateTimeOffset? WorkPlanDeadline { get; set; }
        public string WorkPlanNote { get; set; }
        public bool WorkPlanIsDone { get; set; }
        public DateTime? WorkPlanDoneDate { get; set; }
        public string UserCreated { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}
