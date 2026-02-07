using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // primary constructor // ctor snippet 

    public DbSet<Client> Clients { get; set; }
    public DbSet<AppUser> Users { get; set; }

    public DbSet<Member> Members { get; set; }

    public DbSet<Property> Properties { get; set; }

    public DbSet<ClientSelectionToken> ClientSelectionTokens { get; set; }

    public DbSet<UserClientAccess> UserClientAccess { get; set; }


}
