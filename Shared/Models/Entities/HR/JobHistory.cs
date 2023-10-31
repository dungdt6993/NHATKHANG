using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface JobHistory
    {
        public int JobID { get; set; }

        public int SalaryID { get; set; }

        public DateTimeOffset? JobStartDate { get; set; }

        public DateTimeOffset? StartContractDate { get; set; }

        public DateTimeOffset? EndContractDate { get; set; }
    }
}