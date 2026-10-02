using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder.ToTable("PhoneNumbers");

            builder.HasKey(ph => ph.Id);
            builder.Property(ph => ph.Id);

            builder.Property(ph=>ph.Type)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(ph => ph.Value)
                .HasMaxLength(50)
                .IsRequired();

        }
    }
}
