namespace NEO_SALES.CORE.Models.Dtos.Auth;

public class LoginRequestDto
{
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
