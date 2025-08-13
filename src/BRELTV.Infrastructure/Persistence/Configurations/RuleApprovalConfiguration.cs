using BRELTV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRELTV.Infrastructure.Persistence.Configurations
{
    public class RuleApprovalConfiguration : IEntityTypeConfiguration<RuleApproval>
    {
        public void Configure(EntityTypeBuilder<RuleApproval> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ApprovalStatus)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.RequestedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.ApprovedBy)
                .HasMaxLength(100);

            builder.Property(e => e.Comments)
                .HasMaxLength(500);

            builder.HasOne(e => e.BusinessRule)
                .WithMany()
                .HasForeignKey(e => e.BusinessRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

