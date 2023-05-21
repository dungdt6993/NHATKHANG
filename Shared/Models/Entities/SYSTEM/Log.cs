using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.SYSTEM
{
    interface Log
    {
        public string LogType { get; set; }
        public string LogName { get; set; }
        public string LogDesc { get; set; }
        public DateTime LogTime { get; set; }
        public string LogNote { get; set; }
        public string LogUser { get; set; }
    }
}
