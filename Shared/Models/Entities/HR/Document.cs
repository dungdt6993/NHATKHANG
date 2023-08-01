using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    interface Document
    {
        public int DocID { get; set; }
        public string DocName { get; set; }
        public string TextNumber { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? ExpDate { get; set; }
        public string DocNote { get; set; }
        public string FileScan { get; set; }
        public string isActive { get; set; }
    }
}
