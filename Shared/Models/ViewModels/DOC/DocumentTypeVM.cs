using D69soft.Shared.Models.Entities.DOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.DOC
{
    public class DocumentTypeVM : DocumentType
    {
        //parameter
        public int IsTypeUpdate { get; set; }
        //parameter

        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public int NumExpDate { get; set; }
        public string GroupType { get; set; }
    }
}
