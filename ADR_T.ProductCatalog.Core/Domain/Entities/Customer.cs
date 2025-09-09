using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class Customer : EntityBase
{
    public string UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public ICollection<Order> Orders { get; private set; } = new List<Order>();

    private Customer() { }

    public Customer(string userId, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("Un Customer debe estar siempre asociado a un ApplicationUser.");
        }

        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("El nombre (FirstName) no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("El apellido (LastName) no puede estar vacío.");
        }

        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }

    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainException("El nombre (FirstName) no puede estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainException("El apellido (LastName) no puede estar vacío.");
        }

        FirstName = firstName;
        LastName = lastName;
        FechacActualizacion = DateTime.UtcNow;
    }
}
