using Microsoft.Extensions.Configuration;

namespace ApiCommons.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Returns the configuration value for <paramref name="key"/>, or throws
    /// <see cref="InvalidOperationException"/> if the key is missing or null.
    /// Fills the gap left by <c>GetRequiredSection</c> (sections only) for plain string values.
    /// </summary>
    public static string GetRequired(this IConfiguration configuration, string key)
        => configuration[key] ?? throw new InvalidOperationException(
            $"Required configuration key '{key}' was not found.");
}
