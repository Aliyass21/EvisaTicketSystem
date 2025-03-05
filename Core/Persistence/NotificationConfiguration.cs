using System;
using EVisaTicketSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EVisaTicketSystem.Core.Persistence;
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Primary Key
        builder.HasKey(n => n.Id);

        // Message Configuration
        builder.Property(n => n.Message)
               .IsRequired()
               .HasMaxLength(500); // Limit the message length to 500 characters

        // IsRead Configuration
        builder.Property(n => n.IsRead)
               .IsRequired()
               .HasDefaultValue(false); // Default value for IsRead is false

        // UserId Configuration
        builder.Property(n => n.UserId)
               .IsRequired();

        // Relationship with AppUser
        builder.HasOne(n => n.User)
               .WithMany() // No navigation property on AppUser side
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

    }
}