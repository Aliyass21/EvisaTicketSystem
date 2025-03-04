using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVisaTicketSystem.Core.Persistence;

public class TicketActionConfiguration : IEntityTypeConfiguration<TicketAction>
{
    public void Configure(EntityTypeBuilder<TicketAction> builder)
    {
        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.ActionDate).IsRequired();
        builder.Property(ta => ta.Notes).HasMaxLength(1000);

        builder.HasOne(ta => ta.Ticket)
            .WithMany(t => t.Actions)
            .HasForeignKey(ta => ta.TicketId);

        builder.HasOne(ta => ta.User)
            .WithMany()
            .HasForeignKey(ta => ta.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}