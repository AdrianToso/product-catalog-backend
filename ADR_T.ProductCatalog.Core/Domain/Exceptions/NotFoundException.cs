namespace ADR_T.ProductCatalog.Core.Domain.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) no se encontró.")
    {
    }
}