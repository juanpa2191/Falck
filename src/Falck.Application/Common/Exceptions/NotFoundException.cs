namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist. Translated to an HTTP 404
/// by the API exception-handling middleware.
/// </summary>
public class NotFoundException(string resource, object key)
    : Exception($"{resource} with id '{key}' was not found.");
