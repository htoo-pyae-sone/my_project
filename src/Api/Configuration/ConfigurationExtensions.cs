using Microsoft.Extensions.Configuration;

namespace Api.Configuration;

/// <summary>Provides API-specific configuration setup.</summary>
public static class ConfigurationExtensions
{
    /// <summary>Adds the shared English error-message catalog to application configuration.</summary>
    /// <param name="configuration">Configuration builder used by the API host.</param>
    /// <returns>The same builder so additional configuration can be chained.</returns>
    public static IConfigurationBuilder AddSharedErrorMessages(
        this IConfigurationBuilder configuration
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var messagePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Messages.en.json");
        return configuration.AddJsonFile(messagePath, optional: false, reloadOnChange: false);
    }
}
