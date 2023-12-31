﻿using System;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface Staff
    {
        public DateTimeOffset? JoinDate { get; set; }

        public DateTimeOffset? TerminateDate { get; set; }

        public int Terminated { get; set; }

        public DateTimeOffset? StartDayAL { get; set; }

        public int SalaryByBank { get; set; }

        public string BankAccount { get; set; }

        public int IsPayByMonth { get; set; }

        public int IsPayByDate { get; set; }

        public string ReasonTerminate { get; set; }

        public int TimeAttCode { get; set; }

        public string SocialInsNumber { get; set; }

        public string HealthInsNumber { get; set; }

        public string BankCode { get; set; }

        public string EmailCompany { get; set; }
    }
}