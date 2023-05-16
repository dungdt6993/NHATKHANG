using D69soft.Shared.Models.Entities.CRM;

namespace D69soft.Shared.Models.ViewModels.CRM
{
    public class CustomerVM : Customer
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime Birthday { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }

        //Bien
        public int IsTypeUpdate { get; set; }
    }
}
