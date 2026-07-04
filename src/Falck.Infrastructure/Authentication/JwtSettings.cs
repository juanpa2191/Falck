namespace Falck.Infrastructure.Authentication;

/// <summary>Bound from the "Jwt" configuration section.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// HMAC-SHA256 signing key (minimum 32 bytes). In production this must
    /// come from a secret store (user-secrets, Key Vault, env vars), never
    /// from a committed appsettings file.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
