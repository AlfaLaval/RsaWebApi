using Rsa.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerate
{
    public class RsaContext : DbContext
    {
        public RsaContext() : base($"{AppSettings.Environment}RsaDbConnectionString")
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<ReportHeader> ReportHeaders { get; set; }
        public DbSet<SafetyFirstCheck> SafetyFirstChecks { get; set; }
        public DbSet<SafetyFirstCheckDetail> SafetyFirstCheckDetails { get; set; }
        public DbSet<CustomerEquipmentActivity> CustomerEquipmentActivities { get; set; }
        public DbSet<VibrationAnalysisHeader> VibrationAnalysisHeaders { get; set; }
        public DbSet<VibrationAnalysis> VibrationAnalysis { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<ImageHouse> ImageHouses { get; set; }
        public DbSet<Misc> Miscs { get; set; }
        public DbSet<CommonMaster> CommonMasters { get; set; }
    }
}
