using System;
using System.Collections.Generic;
using System.Text;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface SalaryDef
    {
        public string SalaryType { get; set; }

        public string SalaryTypeName { get; set; }

        public bool isSIns { get; set; }

        public bool isCalcByShift { get; set; }
    }
}
