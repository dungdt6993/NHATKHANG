using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.OP
{
    interface Request
    {
        public string RequestCode { get; set; }
        public string EserialRequest { get; set; }
        public DateTime DateOfRequest { get; set; }
        public string ReasonOfRequest { get; set; }

        public bool isSendDirectManager { get; set; }
        public string DirectManager_Eserial { get; set; }
        public DateTime TimeSendDirectManager { get; set; }

        public bool isSendControlDept { get; set; }
        public string ControlDept_Eserial { get; set; }
        public DateTime TimeSendControlDept { get; set; }

        public bool isSendApprove { get; set; }
        public string Approve_Eserial { get; set; }
        public DateTime TimeSendApprove { get; set; }

        public int SeqRequestDetail { get; set; }
        public float Qty { get; set; }
        public float QtyApproved { get; set; }
        public float QtyHandover { get; set; }
        public string INote { get; set; }
        public string RDNote { get; set; }

    }
}
