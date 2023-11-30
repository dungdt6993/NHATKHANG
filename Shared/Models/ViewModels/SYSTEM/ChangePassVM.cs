using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class ChangePassVM
    {
        public bool isShowNewPass { get; set; }
        public bool isShowComfirmNewPass { get; set; }

        public string Eserial { get; set; }

        public string NewPass { get; set; }

        public string ComfirmNewPass { get; set; }
    }
}
