using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVisaTicketSystem.Core.Persistence;

public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
{
    public void Configure(EntityTypeBuilder<TicketAttachment> builder)
    {
        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.FilePath).IsRequired();
        builder.Property(ta => ta.FileName).HasMaxLength(255);

        builder.HasOne(ta => ta.Ticket)
            .WithMany(t => t.Attachments)
            .HasForeignKey(ta => ta.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
