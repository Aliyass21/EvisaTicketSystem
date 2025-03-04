using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Core.Data
{
   public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, Guid,
    IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>,
    IdentityUserToken<Guid>>(options)
{
    public DbSet<AppUser> Users { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder) 
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        
        builder.ApplyConfiguration(new TicketConfiguration());
        builder.ApplyConfiguration(new TicketActionConfiguration());
        builder.ApplyConfiguration(new TicketAttachmentConfiguration());
        builder.ApplyConfiguration(new TicketTypeConfiguration());
        
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);


       
    }


}

}
