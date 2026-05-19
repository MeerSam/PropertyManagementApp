using System;
using API.Data;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Protocols;

namespace API.Helpers;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Load configuration manually
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build(); 

        var connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlite(connectionString);

        // IMPORTANT: No HttpContext, no TenantProvider here.

        return new AppDbContext(optionsBuilder.Options, new DesignTimeTenantProvider());
    }
}
public class DesignTimeTenantProvider : ITenantService
{
     public string GetCurrentClientId() => "DESIGN_TIME_CLIENT";
    public string GetCurrentUserId() => "DESIGN_TIME_USER";
    public string GetCurrentMemberId() => "DESIGN_TIME_MEMBER";
    public bool HasAccessToClient(string clientId) => true;
}