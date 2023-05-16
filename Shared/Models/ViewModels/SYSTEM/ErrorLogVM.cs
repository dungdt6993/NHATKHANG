using D69soft.Shared.Models.Entities.SYSTEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.SYSTEM
{
    public class ErrorLogVM : ErrorLog
    {
        public string ErrType { get; set; }
        public string ErrMessage { get; set; }
        public DateTime ErrTime { get; set; }
        public string ErrNote { get; set; }
    }
}
