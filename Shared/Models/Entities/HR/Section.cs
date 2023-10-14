using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Section
    {
        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string WorkingLocation { get; set; }
        public string AgreementText_Signature_Name { get; set; }
        public string AgreementText_Signature_Position { get; set; }
        public string TimeAttMachine_SerialLog { get; set; }
    }
}
