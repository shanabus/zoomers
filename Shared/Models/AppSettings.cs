using Microsoft.Extensions.Configuration;

namespace ZoomersClient.Shared.Models
{
    public class AppSettings
    {
        public string StorageConnectionString { get; set; }

        // public static AppSettings LoadAppSettings()
        // {
        //     IConfigurationRoot configRoot = new ConfigurationBuilder()
        //         .AddJsonFile("appsettings.json")
        //         .AddUserSecrets(typeof(AppSettings).Assembly)
        //         .Build();
        //     AppSettings appSettings = configRoot.Get<AppSettings>();
        //     return appSettings;
        // }
    }
}