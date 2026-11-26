using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Application.DTOs.Auth;
using DocForce.Application.Interfaces;
using DocForce.Application.Interfaces.Repositories;
using DocForce.Application.Interfaces.Services;
using DocForce.Domain.Common;
using DocForce.Domain.Errors;

namespace DocForce.Application.Features.Auth.Commands;

public record RefreshTokenCommand(
    string rawRefreshToken) : IRequest<Result<TokenResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepoository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService)
    {
        _userRepoository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }
    public async ValueTask<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByToken(request.rawRefreshToken, cancellationToken);

        if (refreshToken == null)
            return Result.Failure<TokenResponse>(new NotFoundError("RefreshToken.NotFound", "This refresh token don't exist"));

        var user = await _userRepoository.GetById(refreshToken.UserId);
        if (user == null)
            return Result.Failure<TokenResponse>(new NotFoundError("RefreshToken.NotFound", "User don't exist"));

        refreshToken.RevokeToken();

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync();

        return new TokenResponse(newAccessToken, newRefreshToken.Token);
    }
}
