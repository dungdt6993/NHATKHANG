using System;
using D69soft.Shared.Models.Entities.FIN;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class PeriodVM : Period
    {
        public int Period { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
    }
}

