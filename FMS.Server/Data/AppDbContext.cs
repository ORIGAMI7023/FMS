using Microsoft.EntityFrameworkCore;
using FMS.Server.Models;

namespace FMS.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RevenueRecord> RevenueRecords { get; set; }
    }
}
