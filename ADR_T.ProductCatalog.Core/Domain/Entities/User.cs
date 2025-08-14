namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Constructor para EF
    public User() { }

    // Constructor para creación desde dominio
    public User(Guid id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
}
