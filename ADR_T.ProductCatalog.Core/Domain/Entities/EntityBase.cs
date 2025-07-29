namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime FechacCreacion { get; protected set; } = DateTime.UtcNow;
    public DateTime? FechacActualizacion { get; protected set; }
    public bool IsDeleted { get; set; }

    protected EntityBase() { }

    protected EntityBase(Guid id)
    {
        Id = id;
    }
}
