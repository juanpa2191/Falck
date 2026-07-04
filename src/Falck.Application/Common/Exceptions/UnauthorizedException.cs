namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Thrown when credentials are invalid. Translated to an HTTP 401 by the API
/// middleware. The message is deliberately generic: never reveal whether the
/// username or the password was wrong.
/// </summary>
public class UnauthorizedException(string message) : Exception(message);
