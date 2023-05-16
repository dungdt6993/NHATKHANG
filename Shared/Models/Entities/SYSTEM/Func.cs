namespace D69soft.Shared.Models.Entities.SYSTEM
{
    public interface Func
    {
        public int FNo { get; set; }

        public string FuncID { get; set; }

        public string FuncName { get; set; }

        public string FuncURL { get; set; }

        public bool isActive { get; set; }
    }
}