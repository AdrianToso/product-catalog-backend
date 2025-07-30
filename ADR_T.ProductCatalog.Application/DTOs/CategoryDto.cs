namespace ADR_T.ProductCatalog.Application.DTOs;
public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; set; }
}