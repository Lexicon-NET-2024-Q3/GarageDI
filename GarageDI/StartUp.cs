using GarageDI.Services;

namespace GarageDI;

internal class StartUp
{
    internal void SetUp()
    {
        ServiceCollection serviceCollection = new();
        ConfigureServices(serviceCollection);
        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        try
        {
            var garageManager = serviceProvider.GetRequiredService<GarageManager>();
            garageManager.Run();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error: {e.Message}\nStack Trace: {e.StackTrace}");
            throw;
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        IConfiguration configuration = GetConfig();

        services.AddSingleton(configuration);
        services.AddSingleton<ISettings>(configuration.GetSection(ConfigHelpers.GetSettingsFromConfig).Get<Settings>());
        services.AddSingleton<GarageManager>();
        services.AddSingleton<IGarage<IVehicle>, InMemoryGarage<IVehicle>>();
        services.AddSingleton<IGarageHandler, GarageHandler>();
        services.AddTransient<IUI, ConsoleUI>();
        services.AddSingleton<IUtil, Util>();
        services.AddSingleton<IGaragePersistenceService, GaragePersistenceService>();
    }

    private static IConfigurationRoot GetConfig()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(ConfigHelpers.AppSettingsFileName, optional: true, reloadOnChange: true)
            .Build();
    }

}