using BRELTV.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRELTV.Infrastructure.Persistence.Configurations
{
    public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
    {
        public void Configure(EntityTypeBuilder<CustomerProfile> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ProfileBand)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            builder.HasIndex(e => e.ProfileBand)
                .IsUnique();
        }
    }
}

