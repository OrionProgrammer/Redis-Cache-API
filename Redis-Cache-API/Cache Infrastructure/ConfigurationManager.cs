namespace Redis_Cache_API.Cache_Infrastructure;

static class ConfigurationManager
{
    public static IConfiguration AppSetting { get; }
    static ConfigurationManager()
    {
        AppSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json").Build();
    }
}
