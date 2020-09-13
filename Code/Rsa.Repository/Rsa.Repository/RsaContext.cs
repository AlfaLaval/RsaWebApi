using Microsoft.EntityFrameworkCore;
using Rsa.Models.DbEntities;
using System;

namespace Rsa.Repository
{
    public class RsaContext : DbContext
    {
        public RsaContext(DbContextOptions<RsaContext> options) : base(options)
        {

        }
        public DbSet<ReportHeader> ReportHeaders { get; set; }
        public DbSet<SafetyFirstCheck> SafetyFirstChecks { get; set; }
        public DbSet<SafetyFirstCheckDetail> SafetyFirstCheckDetails { get; set; }
    }
}
