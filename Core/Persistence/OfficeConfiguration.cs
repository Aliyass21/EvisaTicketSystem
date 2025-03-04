using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVisaTicketSystem.Core.Persistence;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.OfficeType)
            .IsRequired()
            .HasConversion<string>(); 
    }
}
