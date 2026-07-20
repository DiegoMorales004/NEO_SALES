using NEO_SALES.CORE.Models.Dtos.Auth;

namespace NEO_SALES.CORE.Interfaces.Repositories;

public interface IAuthRepository
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
