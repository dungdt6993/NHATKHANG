﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface CruiseStatus
    {
        public string CruiseStatusCode { get; set; }
        public string CruiseStatusName { get; set; }
        public int NumDay { get; set; }
        public string ColorHEX { get; set; }
    }
}
