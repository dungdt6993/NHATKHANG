using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface LockDutyRoster
    {
        public DateTimeOffset? LockFrom { get; set; }
        public DateTimeOffset? LockTo { get; set; }
        public string EserialLock { get; set; }
        public DateTime TimeLock { get; set; }
    }
}
