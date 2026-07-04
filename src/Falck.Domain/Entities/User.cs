namespace Falck.Domain.Entities;

/// <summary>
/// API account used for authentication. Kept as a lean custom table instead of
/// full ASP.NET Identity: the test only needs two roles and JWT issuance.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt hash; the plain password is never stored.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>One of <see cref="Roles"/>.</summary>
    public string Role { get; set; } = Roles.User;
}

/// <summary>Authorization roles supported by the API (section 3.2).</summary>
public static class Roles
{
    /// <summary>Full access to every employees endpoint.</summary>
    public const string Admin = "Admin";

    /// <summary>Read-only access (GET) to the employees entity.</summary>
    public const string User = "User";
}
