using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class Address : EntityBase
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    public bool IsPrimary { get; private set; }

    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;

    private Address() { }

    public Address(Guid customerId, string street, string city, string state, string postalCode, string country, bool isPrimary = false)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("La dirección debe estar asociada a un cliente.");
        }

        SetDetails(street, city, state, postalCode, country);
        CustomerId = customerId;
        IsPrimary = isPrimary;
    }

    public void Update(string street, string city, string state, string postalCode, string country)
    {
        SetDetails(street, city, state, postalCode, country);
        FechacActualizacion = DateTime.UtcNow;
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
        FechacActualizacion = DateTime.UtcNow;
    }

    public void UnsetAsPrimary()
    {
        IsPrimary = false;
        FechacActualizacion = DateTime.UtcNow;
    }

    private void SetDetails(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street)) throw new DomainException("La calle (Street) es requerida.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("La ciudad (City) es requerida.");
        if (string.IsNullOrWhiteSpace(state)) throw new DomainException("El estado/provincia (State) es requerido.");
        if (string.IsNullOrWhiteSpace(postalCode)) throw new DomainException("El código postal (PostalCode) es requerido.");
        if (string.IsNullOrWhiteSpace(country)) throw new DomainException("El país (Country) es requerido.");

        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}
