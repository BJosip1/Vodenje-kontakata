using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class ContactTagConfiguration : IEntityTypeConfiguration<ContactTag>
    {
        public void Configure(EntityTypeBuilder<ContactTag> builder)
        {
            builder.ToTable("ContactTags");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id);

            builder.HasIndex(ct => new { ct.ContactId, ct.TagId }).IsUnique();

            builder.HasOne(ct => ct.Contact)
                .WithMany(c => c.ContactTags)
                .HasForeignKey(ct => ct.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.Tag)
                .WithMany(t => t.ContactTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
