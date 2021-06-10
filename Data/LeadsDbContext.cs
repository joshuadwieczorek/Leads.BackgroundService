using Microsoft.EntityFrameworkCore;
using Leads.BackgroundService.Data.Models;

namespace Leads.BackgroundService.Data
{
    public class LeadsDbContext : DbContext
    {
        public DbSet<Token> Tokens { get; set; }
        public DbSet<LeadProvider> LeadProviders { get; set; }
        public DbSet<HttpLog> HttpLogs { get; set; }
        public DbSet<HttpLogData> HttpLogData { get; set; }
        public DbSet<QueueStatus> QueueStatuses { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<Adf> Adfs { get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LeadsDbContext(DbContextOptions<LeadsDbContext> options) : base(options) { }
    }
}