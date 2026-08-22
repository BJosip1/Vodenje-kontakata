using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id);

            builder.Property(a => a.Street)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.PostalCode)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(a => a.Country)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
