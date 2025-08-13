using BRELTV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRELTV.Infrastructure.Persistence.Configurations
{
    public class BusinessRuleConfiguration : IEntityTypeConfiguration<BusinessRule>
    {
        public void Configure(EntityTypeBuilder<BusinessRule> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.NoIncomeProofLTV)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(e => e.MaxLTVWithProof)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(e => e.FIRequirement)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.MinIncomeProofAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(e => e.MinFloatingMoneyPercentage)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            builder.Property(e => e.ApprovedBy)
                .HasMaxLength(100);

            builder.HasOne(e => e.CustomerProfile)
                .WithMany(e => e.BusinessRules)
                .HasForeignKey(e => e.CustomerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

