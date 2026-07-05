namespace Falck.Application.DTOs.Auth;

/// <summary>El JWT emitido más el contexto que el cliente necesita para usarlo.</summary>
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
