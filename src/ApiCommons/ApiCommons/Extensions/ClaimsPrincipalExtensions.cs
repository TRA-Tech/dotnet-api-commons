using System.Security.Claims;

namespace ApiCommons.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Parses the <c>NameIdentifier</c> claim as <typeparamref name="T"/>.
    /// Returns <c>default</c> when the claim is absent or cannot be parsed.
    /// </summary>
    public static T? GetId<T>(this ClaimsPrincipal user) where T : IParsable<T>
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return value is not null && T.TryParse(value, null, out var id) ? id : default;
    }

    /// <summary>
    /// Parses the <c>NameIdentifier</c> claim as <typeparamref name="T"/>.
    /// Throws <see cref="InvalidOperationException"/> when the claim is absent or cannot be parsed.
    /// </summary>
    public static T GetRequiredId<T>(this ClaimsPrincipal user) where T : IParsable<T>
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value is not null && T.TryParse(value, null, out var id))
            return id;
        throw new InvalidOperationException("NameIdentifier claim is missing or could not be parsed.");
    }

    /// <summary>Returns the <c>Email</c> claim value, or <c>null</c> if absent.</summary>
    public static string? GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Email)?.Value;
}
