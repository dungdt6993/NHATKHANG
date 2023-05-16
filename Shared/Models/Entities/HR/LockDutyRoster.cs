using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface LockDutyRoster
    {
        public DateTime? LockFrom { get; set; }
        public DateTime? LockTo { get; set; }
        public string EserialLock { get; set; }
        public DateTime TimeLock { get; set; }
    }
}
