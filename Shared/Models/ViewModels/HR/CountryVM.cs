using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class CountryVM : Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
    }
}
