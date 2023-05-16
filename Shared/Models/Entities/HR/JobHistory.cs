using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface JobHistory
    {
        public int JobID { get; set; }

        public int SalaryID { get; set; }

        public DateTime? JobStartDate { get; set; }

        public DateTime? StartContractDate { get; set; }

        public DateTime? EndContractDate { get; set; }
    }
}