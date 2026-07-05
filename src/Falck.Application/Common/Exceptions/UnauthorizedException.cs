namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando las credenciales son inválidas. El middleware de la API la
/// traduce a un HTTP 401. El mensaje es deliberadamente genérico: nunca revela si
/// lo incorrecto fue el usuario o la contraseña.
/// </summary>
public class UnauthorizedException(string message) : Exception(message);
