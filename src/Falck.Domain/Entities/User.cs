namespace Falck.Domain.Entities;

/// <summary>
/// Cuenta de la API usada para autenticación. Se mantiene como una tabla propia
/// y ligera en lugar de ASP.NET Identity completo: la prueba solo necesita dos
/// roles y emisión de JWT.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>Hash BCrypt; la contraseña en texto plano nunca se almacena.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Uno de <see cref="Roles"/>.</summary>
    public string Role { get; set; } = Roles.User;
}

/// <summary>Roles de autorización soportados por la API (sección 3.2).</summary>
public static class Roles
{
    /// <summary>Acceso total a todos los endpoints de empleados.</summary>
    public const string Admin = "Admin";

    /// <summary>Acceso de solo lectura (GET) a la entidad de empleados.</summary>
    public const string User = "User";
}
