namespace Falck.Infrastructure.Authentication;

/// <summary>Enlazado desde la sección de configuración "Jwt".</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Clave de firma HMAC-SHA256 (mínimo 32 bytes). En producción debe provenir
    /// de un almacén de secretos (user-secrets, Key Vault, variables de entorno),
    /// nunca de un archivo appsettings commiteado.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public int ExpiryMinutes { get; set; } = 60;
}
