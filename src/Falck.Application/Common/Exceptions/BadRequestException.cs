namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando una petición es semánticamente inválida (p. ej. referencia un
/// departamento inexistente). El middleware de la API la traduce a un HTTP 400.
/// </summary>
public class BadRequestException(string message) : Exception(message);
