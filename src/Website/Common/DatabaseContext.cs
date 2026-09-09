using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website.Models.Database;

namespace Website.Common;

public class DatabaseContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<ShortLink> ShortLinks { get; set; }

    public DbSet<ShortLinkHit> ShortLinkHits { get; set; }

    public DbSet<Image> Images { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;


    public DatabaseContext()
    { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}
