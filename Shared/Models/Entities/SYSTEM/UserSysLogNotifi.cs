using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.SYSTEM
{
    public interface UserSysLogNotifi
    {
        public int USLNSeq { get; set; }

        public string UserNotifi { get; set; }
        public int isSeen { get; set; }

        public DateTime? TimeSeen { get; set; }

        public int isNotifi { get; set; }

        public int isNotifiMention { get; set; }
    }
}
