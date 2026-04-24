namespace GoodHamburger.Core.Exceptions;

/// <summary>Violação de regra de negócio — mapeada para 422 Unprocessable Entity.</summary>
public sealed class DomainException(string message) : Exception(message);

/// <summary>Recurso não encontrado — mapeada para 404 Not Found.</summary>
public sealed class NotFoundException(string resource, object key)
    : Exception($"'{resource}' com identificador '{key}' não foi encontrado.");
