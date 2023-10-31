using D69soft.Shared.Models.Entities.HR;
using D69soft.Shared.Models.Entities.OP;
using Model.Entities.OP;

namespace Model.ViewModels.OP
{
    public class VehicleScheduleVM : Vehicle, VehicleSchedule, Shift
    {
        public string VehicleCode { get; set; }
        public string VehicleName { get; set; }
        public bool VehicleActive { get; set; }
        public DateTime dDate { get; set; }
        public bool VehicleStatus { get; set; }
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTimeOffset? BeginTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }
    }
}
