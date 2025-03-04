using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVisaTicketSystem.Core.Persistence;

public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.Title).IsRequired().HasMaxLength(100);
        builder.Property(tt => tt.Description).HasMaxLength(500);
    }
}
