using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface ALMonth
    {
        public int ALCurrentYear { get; set; }
        public int NumMonthAL { get; set; }
        public decimal ALPreMonth { get; set; }
        public decimal ALExperience { get; set; }
        public int ALActive { get; set; }
        public decimal ALAddBalance { get; set; }
        public decimal ALTotal { get; set; }
        public decimal ALTaken { get; set; }
        public decimal ALBalance { get; set; }
        public string ALNoteAddBalance { get; set; }
    }
}
