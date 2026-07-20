using NEO_SALES.CORE.Models.Dtos.Auth;

namespace NEO_SALES.CORE.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
