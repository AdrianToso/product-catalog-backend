namespace ADR_T.ProductCatalog.Application.DTOs.Auth;
public class AuthResultDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }

    public AuthResultDto() { }

    public AuthResultDto(string token)
    {
        Token = token;
        ExpiresAt = DateTime.UtcNow.AddHours(1);
    }
}
