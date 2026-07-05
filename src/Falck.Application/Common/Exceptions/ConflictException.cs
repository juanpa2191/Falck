namespace Falck.Application.Common.Exceptions;

/// <summary>
/// Se lanza cuando una petición entra en conflicto con el estado actual del
/// recurso (p. ej. una actualización/eliminación concurrente, o una violación de
/// unicidad perdida por una condición de carrera). El middleware de manejo de
/// excepciones de la API la traduce a un HTTP 409.
/// </summary>
public class ConflictException(string message) : Exception(message);
