using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Shift
    {
        public string ShiftID { get; set; }
        public string ShiftName { get; set; }
        public DateTimeOffset? BeginTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string ColorHEX { get; set; }
        public bool isNight { get; set; }
        public bool isSplit { get; set; }

    }
}
