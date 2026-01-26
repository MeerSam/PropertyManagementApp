using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
// primary constructor // ctor snippet 
public DbSet<AppUser> Users { get; set; }


}
