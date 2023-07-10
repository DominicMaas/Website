﻿using Microsoft.EntityFrameworkCore;
using Website.Models.Database;

namespace Website.Common;

public class DatabaseContext : DbContext
{
    public DbSet<ShortLink> ShortLinks { get; set; }

    public DatabaseContext()
    { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}
