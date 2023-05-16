using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface SalaryHistory
    {
        public decimal BasicSalary { get; set; }
        public decimal OtherSalary { get; set; }
        public decimal Benefit1 { get; set; }
        public decimal Benefit2 { get; set; }
        public decimal Benefit3 { get; set; }
        public decimal Benefit4 { get; set; }
        public decimal Benefit5 { get; set; }
        public decimal Benefit6 { get; set; }
        public decimal Benefit7 { get; set; }
        public decimal Benefit8 { get; set; }
        public DateTime? BeginSalaryDate { get; set; }
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
    }
}