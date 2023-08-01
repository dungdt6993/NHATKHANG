using D69soft.Shared.Models.Entities.FIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.ViewModels.FIN
{
    public class RoomTableVM : RoomTable, Invoice
    {
        public string RoomTableID { get; set; }
        public string RoomTableName { get; set; }
        public int isOpen { get; set; }
        public int isClose { get; set; }
        public string OpenBy { get; set; }
        public string CheckNo { get; set; }
        public DateTime? IDate { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public string CloseBy { get; set; }
        public decimal Invoice_DiscountPrice { get; set; }
        public decimal Invoice_DiscountPercent { get; set; }
        public decimal Invoice_TaxPercent { get; set; }
        public decimal Invoice_TotalPaid { get; set; }
        public bool INVActive { get; set; }

        //Bien
        public string OpenByName { get; set; }
    }
}
