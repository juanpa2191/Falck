namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando un recurso solicitado no existe. El middleware de manejo de
/// excepciones de la API la traduce a un HTTP 404.
/// </summary>
public class NotFoundException(string resource, object key)
    : Exception($"{resource} with id '{key}' was not found.");
