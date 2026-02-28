using System;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext(DbContextOptions options, ITenantService tenantService) : DbContext(options)
{
    // primary constructor // ctor snippet 
    public bool IsSeeding { get; set; }


    public DbSet<Client> Clients { get; set; }
    public DbSet<AppUser> Users { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<ClientSelectionToken> ClientSelectionTokens { get; set; }

    public DbSet<UserClientAccess> UserClientAccess { get; set; }

    public DbSet<PropertyOwnership> PropertyOwnerships { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserClientAccess>()
            .HasKey(uca => new { uca.UserId, uca.ClientId }); // Composite PK — no duplicate User+Client combinations possible

        // Apply a Global Query Filter (EF Core / ORM Level)
        if (!IsSeeding)
        {
            builder.Entity<UserClientAccess>().HasQueryFilter(uca => !IsSeeding && uca.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Property>().HasQueryFilter(p => !IsSeeding && p.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Member>().HasQueryFilter(m => !IsSeeding && m.ClientId == tenantService.GetCurrentClientId());
        }


        builder.Entity<UserClientAccess>()
            .HasOne(uca => uca.User)
            .WithMany(u => u.ClientAccess)
            .HasForeignKey(uca => uca.UserId)
            .OnDelete(DeleteBehavior.Restrict); //- Should not be deleted when AppUser is disabled.


        builder.Entity<UserClientAccess>()
            .HasOne(uca => uca.Client)
            .WithMany(c => c.UsersAccess)
            .HasForeignKey(uca => uca.ClientId)
            .OnDelete(DeleteBehavior.Cascade); //- Should be deleted when the Client is deleted.


        // Member → Client (required)

        builder.Entity<Member>()
            .HasOne(m => m.Client)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.ClientId)
            .OnDelete(DeleteBehavior.Cascade); //- Should be deleted when the Client is deleted.


        // Member → AppUser (optional)
        builder.Entity<Member>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);//- Should not be deleted when AppUser is disabled.



        builder.Entity<Property>()
            .HasOne(p => p.Client)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Cascade);  //- Should be deleted when the Client is deleted.


        builder.Entity<PropertyOwnership>()
           .HasOne(po => po.Property)
           .WithMany(p => p.Ownerships)
           .HasForeignKey(po => po.PropertyId)
           .OnDelete(DeleteBehavior.Restrict);  //- Should not be deleted when Property is deleted.

        builder.Entity<PropertyOwnership>()
            .HasOne(po => po.Member)
            .WithMany(m => m.PropertyOwnerships)
            .HasForeignKey(po => po.MemberId)
            .OnDelete(DeleteBehavior.Restrict);//- Should not be deleted when Member is deleted.

        // One Primary owner at a time per property — CoOwners are unrestricted
        builder.Entity<PropertyOwnership>()
            .HasIndex(po => new { po.PropertyId, po.OwnershipType, po.IsCurrent })
            .IsUnique()
            .HasFilter("\"IsCurrent\" = true AND \"OwnershipType\" = 0"); // 0 = Primary
    }

}
