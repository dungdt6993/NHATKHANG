namespace D69soft.Shared.Models.Entities.HR
{
    public interface Division
    {
        public string DivisionID { get; set; }

        public string DivisionName { get; set; }
        public string DivisionShortName { get; set; }

        public string CodeDivs { get; set; }

        public string DivsAddress { get; set; }

        public string DivsTel { get; set; }

        public bool isAutoEserial { get; set; }

        public int is2625 { get; set; }

        public int INOUTNumber { get; set; }

    }
}