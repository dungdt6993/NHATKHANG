using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface CLDOMonth
    {
        public decimal CLDOPreMonth { get; set; }
        public decimal CLDOTransfer { get; set; }
        public decimal CLDOAddBalance { get; set; }
        public decimal CLDOTotal { get; set; }
        public decimal CLDOTaken { get; set; }
        public decimal CLDOBalance { get; set; }
        public string CLDONoteAddBalance { get; set; }
    }
}
