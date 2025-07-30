namespace ADR_T.ProductCatalog.Application.DTOs;

public record ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<string> Categories { get; set; } = new List<string>();
}
