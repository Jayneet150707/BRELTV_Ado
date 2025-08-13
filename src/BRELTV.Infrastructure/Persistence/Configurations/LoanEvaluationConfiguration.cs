using BRELTV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRELTV.Infrastructure.Persistence.Configurations
{
    public class LoanEvaluationConfiguration : IEntityTypeConfiguration<LoanEvaluation>
    {
        public void Configure(EntityTypeBuilder<LoanEvaluation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CustomerProfileBand)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.IncomeProofAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.FloatingMoneyAfterExpensesAndEMI)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(e => e.AssignedLTV)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(e => e.FIRequirement)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Reason)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.EvaluatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(e => e.EvaluatedAt);
        }
    }
}

