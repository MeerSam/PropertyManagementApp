using System;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

// public class AppDbContext(DbContextOptions options, ITenantService tenantService) : DbContext(options)
public class AppDbContext(DbContextOptions options, ITenantService tenantService) : IdentityDbContext<AppUser>(options)

{
    // primary constructor // ctor snippet 
    public bool IsSeeding { get; set; }


    public DbSet<Client> Clients { get; set; }
    // public DbSet<AppUser> Users { get; set; } // User Table will be created by Identity

    public DbSet<Member> Members { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<ClientSelectionToken> ClientSelectionTokens { get; set; }

    public DbSet<UserClientAccess> UserClientAccess { get; set; }

    public DbSet<PropertyOwnership> PropertyOwnerships { get; set; }

    public DbSet<Document> Documents { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Seed SuperAdmin role
        builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole { Id = "superadmin-id", Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
            new IdentityRole { Id = "support-id", Name = "Support", NormalizedName = "SUPPORT" },
            new IdentityRole { Id = "sysadmin-id", Name = "SysAdmin", NormalizedName = "SYSADMIN" },
            new IdentityRole { Id = "appuser-id", Name = "AppUser", NormalizedName = "APPUSER" }

        );
 

        builder.Entity<UserClientAccess>()
            .HasKey(uca => new { uca.UserId, uca.ClientId }); // Composite PK — no duplicate User+Client combinations possible

        // Apply a Global Query Filter (EF Core / ORM Level)
        if (!IsSeeding)
        {
            builder.Entity<UserClientAccess>().HasQueryFilter(uca => !IsSeeding && uca.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Property>().HasQueryFilter(p => !IsSeeding && p.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Member>().HasQueryFilter(m => !IsSeeding && m.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Vehicle>().HasQueryFilter(v => v.ClientId == tenantService.GetCurrentClientId());
            builder.Entity<Vehicle>().HasQueryFilter(v => v.ClientId == tenantService.GetCurrentClientId());
            // EF Core captures the method call(tenantService.GetCurrentClientId()) inside the expression tree and evaluates it later — not during model creation.
            // This prevents the “No active HTTP context” crash if you create a var clientID =tenantService.GetCurrentClientId() outside the expression.
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


        // Member -> Client (required)

        builder.Entity<Member>()
            .HasOne(m => m.Client)
            .WithMany(c => c.Members)
            .HasForeignKey(m => m.ClientId)
            .OnDelete(DeleteBehavior.Cascade); //- Should be deleted when the Client is deleted.


        // Member -> AppUser (optional)
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

        // Tenant query filter — same pattern as Property/Member
        if (!IsSeeding)
        {
            builder.Entity<Document>()
                .HasQueryFilter(d =>
                    !IsSeeding &&
                    d.ClientId == tenantService.GetCurrentClientId() &&
                    d.IsActive);
        }

        // Document -> Client
        builder.Entity<Document>()
            .HasOne(d => d.Client)
            .WithMany()
            .HasForeignKey(d => d.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Document -> Property (optional)
        builder.Entity<Document>()
            .HasOne(d => d.Property)
            .WithMany()
            .HasForeignKey(d => d.PropertyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Document -> PropertyOwnership (optional)
        builder.Entity<Document>()
            .HasOne(d => d.PropertyOwnership)
            .WithMany()
            .HasForeignKey(d => d.PropertyOwnershipId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Document -> UploadedBy
        builder.Entity<Document>()
            .HasOne(d => d.UploadedBy)
            .WithMany()
            .HasForeignKey(d => d.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referential versioning chain
        builder.Entity<Document>()
            .HasOne(d => d.SupersededBy)
            .WithMany()
            .HasForeignKey(d => d.SupersededById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique index: no duplicate StorageKey
        builder.Entity<Document>()
            .HasIndex(d => d.StorageKey)
            .IsUnique();

        builder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);

            entity.HasOne(v => v.Owner)
                .WithMany(m => m.Vehicles)
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(v => v.Client)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Photo>()
        .HasOne(p => p.Member)
        .WithMany(m => m.Photos)
        .HasForeignKey(p => p.MemberId)
        .OnDelete(DeleteBehavior.Cascade);

        /* var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        // for Message we've used Optional Datetime (i.e. DateRead is Datetime?)
        // for the compiler its not the same that is why the DateRead was not returning a UTC Time
        // therefore we must do the same as above for optional DateTime
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        } */

    }

}
