using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contacts");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id);

            builder.Property(c => c.FirstName)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("Croatian_CI_AS");

            builder.Property(c => c.LastName)
                .HasMaxLength(100)
                .IsRequired()
                .UseCollation("Croatian_CI_AS");

            builder.Property(c => c.Note)
                .HasMaxLength(1000)
               .IsRequired(false);

            builder.HasOne(c => c.Address)
                .WithMany(a=>a.Contacts)
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.PhoneNumbers)
                .WithOne(ph => ph.Contact)
                .HasForeignKey(ph => ph.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Emails)
                .WithOne(e => e.Contact)
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
