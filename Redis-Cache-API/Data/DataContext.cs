using Microsoft.EntityFrameworkCore;

namespace Redis_Cache_API.Data;

public class DataContext : DbContext
{
    protected readonly IConfigurationRoot configuration;

    public DataContext()
    {
        configuration = new ConfigurationBuilder()
        .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlServer(configuration.GetConnectionString("Database"));

    public DbSet<Picture> Picture { get; set; }

}
