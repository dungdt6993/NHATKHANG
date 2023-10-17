using System;
using D69soft.Shared.Models.Entities.HR;

namespace D69soft.Shared.Models.ViewModels.HR
{
    public class SectionVM : Section, Division
    {
        //Para
        public bool isActive { get; set; }
        public int IsTypeUpdate { get; set; }

        //Para

        public string SectionID { get; set; }
        public string SectionName { get; set; }
        public string WorkingLocation { get; set; }
        public string AgreementText_Signature_Name { get; set; }
        public string AgreementText_Signature_Position { get; set; }
        public string TimeAttMachine_SerialLog { get; set; }
        public string DivisionID { get; set; }
        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }
        public string DivisionTaxCode { get; set; }
        public string CodeDivs { get; set; }
        public string DivisionAddress { get; set; }
        public string DivisionTel { get; set; }
        public string DivisionHotline { get; set; }
        public string DivisionEmail { get; set; }
        public string DivisionWebsite { get; set; }
        public string DivisionBankAccount { get; set; }
        public string DivisionBankName { get; set; }
        public string DivisionLogoUrl { get; set; }
        public bool isAutoEserial { get; set; }
        public int is2625 { get; set; }
        public int INOUTNumber { get; set; }
    }
}
