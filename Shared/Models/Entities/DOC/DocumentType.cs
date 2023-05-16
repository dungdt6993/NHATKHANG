using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.DOC
{
    interface DocumentType
    {
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public int NumExpDate { get; set; }
        public string GroupType { get; set; }
    }
}
