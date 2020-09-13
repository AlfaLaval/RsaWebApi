using Microsoft.EntityFrameworkCore;
using Rsa.Models.DbEntities;

namespace Rsa.Services
{
    public class RsaContext : DbContext
    {
        public RsaContext(DbContextOptions<RsaContext> options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<ReportHeader> ReportHeaders { get; set; }
        public DbSet<SafetyFirstCheck> SafetyFirstChecks { get; set; }
        public DbSet<SafetyFirstCheckDetail> SafetyFirstCheckDetails { get; set; }
        public DbSet<CustomerEquipmentActivity> CustomerEquipmentActivities { get; set; }
        public DbSet<VibrationAnalysisHeader> VibrationAnalysisHeaders { get; set; }
        public DbSet<VibrationAnalysis> VibrationAnalysis { get; set; }
    }
}
