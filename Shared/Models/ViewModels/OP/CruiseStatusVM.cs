using D69soft.Shared.Models.Entities.OP;

namespace D69soft.Shared.Models.ViewModels.OP
{
    public class CruiseStatusVM : CruiseStatus
    {
        public string CruiseStatusCode { get; set; }
        public string CruiseStatusName { get; set; }
        public int NumDay { get; set; }
        public string ColorHEX { get; set; }
    }
}
