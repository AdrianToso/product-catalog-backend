namespace ADR_T.ProductCatalog.Core.Domain.Exceptions;

// Excepción de dominio para cuando ocurren fallos de validación.
public class ValidationException : Exception
{
    public ValidationException()
        : base("Existe uno o mas errores de validacion.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}