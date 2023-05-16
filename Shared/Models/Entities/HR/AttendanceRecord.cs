using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D69soft.Shared.Models.Entities.HR
{
    public interface AttendanceRecord
    {
        public int ARID { get; set; }
        public DateTime dDate { get; set; }
        public DateTime? IN1 { get; set; }
        public DateTime? OUT1 { get; set; }
        public DateTime? IN2 { get; set; }
        public DateTime? OUT2 { get; set; }
        public DateTime? IN3 { get; set; }
        public DateTime? OUT3 { get; set; }
        public DateTime? IN4 { get; set; }
        public DateTime? OUT4 { get; set; }
        public int OUT1_isNS { get; set; }
        public int OUT2_isNS { get; set; }
        public int OUT3_isNS { get; set; }
        public int OUT4_isNS { get; set; }


        public string Explain { get; set; }
        public string EserialExplain { get; set; }
        public DateTime? ExplainTime { get; set; }

        public string ExplainHOD { get; set; }
        public string EserialExplainHOD { get; set; }
        public DateTime? ExplainHODTime { get; set; }

        public string ExplainHR { get; set; }
        public string EserialExplainHR { get; set; }
        public DateTime? ExplainHRTime { get; set; }

        public bool isConfirmExplain { get; set; }
        public string EserialConfirmExplain { get; set; }
        public DateTime? TimeConfirmExplain { get; set; }


        public bool isConfirmLateSoon { get; set; }
        public string EserialConfirmLateSoon { get; set; }
        public DateTime? TimeConfirmLateSoon { get; set; }

    }
}
