namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Thrown when a request is semantically invalid (e.g. references a
/// non-existent department). Translated to an HTTP 400 by the API middleware.
/// </summary>
public class BadRequestException(string message) : Exception(message);
