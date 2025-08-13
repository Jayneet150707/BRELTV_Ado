using BRELTV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BRELTV.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BusinessRule> BusinessRules { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<RuleApproval> RuleApprovals { get; set; }
        public DbSet<LoanEvaluation> LoanEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}

