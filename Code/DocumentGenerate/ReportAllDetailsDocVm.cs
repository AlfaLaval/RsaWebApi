using Rsa.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerate
{
    class ReportAllDetailsDocVm
    {
        public ReportHeader ReportHeader { get; set; }
        public SafetyFirstCheck SafetyFirstCheck { get; set; }
        public CustomerEquipmentActivity CustomerEquipmentActivity { get; set; }
        public VibrationAnalysisHeader VibrationAnalysisHeader { get; set; }
        public List<Observation> Observations { get; set; }
        public List<Recommendation> Recommendations { get; set; }
        public Misc Misc { get; set; }
    }
}
