using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface CLPHMonth
    {
        public decimal CLPHPreMonth { get; set; }
        public decimal CLPHTransfer { get; set; }
        public decimal CLPHAddBalance { get; set; }
        public decimal CLPHTotal { get; set; }
        public decimal CLPHTaken { get; set; }
        public decimal CLPHBalance { get; set; }
        public string CLPHNoteAddBalance { get; set; }
    }
}
