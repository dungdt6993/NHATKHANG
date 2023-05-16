using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class AdjustProfileRptVM : AdjustProfile, Rpt
    {
        public string AdjustProfileID { get; set; }
        public string AdjustProfileName { get; set; }
        public int RptID { get; set; }
        public string RptName { get; set; }
        public string RptUrl { get; set; }
        public bool PassUserID { get; set; }
    }
}
