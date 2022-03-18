using Microsoft.EntityFrameworkCore;
using WeatherArchive.Models;

namespace WeatherArchive.Data
{
    /// <summary>
    /// Database context for weather archives.
    /// </summary>
    public class WeatherArchiveContext : DbContext
    {
        public DbSet<ArchiveEntry> ArchiveEntries { get; private set; }

        public WeatherArchiveContext(DbContextOptions<WeatherArchiveContext> options)
            : base(options)
        {}
    }
}
