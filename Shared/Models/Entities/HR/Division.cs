namespace D69soft.Shared.Models.Entities.HR
{
    public interface Division
    {
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