using D69soft.Shared.Models.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class AdjustProfileVM : AdjustProfile
    {
        public string AdjustProfileID { get; set; }
        public string AdjustProfileName { get; set; }

        //Bien
        public int[] arrRptID { get; set; } = new int[] { };
        public string strRpt { get; set; }
    }
}
