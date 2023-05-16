using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.CRM
{
    internal interface Customer
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public DateTime Birthday { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
    }
}
