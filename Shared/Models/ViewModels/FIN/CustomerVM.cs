using D69soft.Shared.Models.Entities.FIN;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class CustomerVM : Customer
    {
        public int IsTypeUpdate { get; set; }

        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTaxCode { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public string CustomerTel { get; set; }
        public string CustomerAddress { get; set; }

    }
}
