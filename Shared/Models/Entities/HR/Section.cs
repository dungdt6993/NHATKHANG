using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Section
    {
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string WorkingLocation { get; set; }
    }
}
