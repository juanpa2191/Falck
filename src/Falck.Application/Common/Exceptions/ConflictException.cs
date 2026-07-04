namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Thrown when a request conflicts with the current state of the resource
/// (e.g. a concurrent update/delete, or a uniqueness violation lost to a race).
/// Translated to an HTTP 409 by the API exception-handling middleware.
/// </summary>
public class ConflictException(string message) : Exception(message);
