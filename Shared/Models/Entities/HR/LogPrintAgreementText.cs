using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface LogPrintAgreementText
    {
        public int Seq { get; set; }
        public DateTime dDate { get; set; }
        public int isPrint { get; set; }
        public string EserialPrint { get; set; }
        public DateTime? TimePrint { get; set; }
    }
}
