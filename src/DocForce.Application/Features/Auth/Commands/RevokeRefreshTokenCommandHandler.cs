using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Application.Interfaces;
using DocForce.Application.Interfaces.Repositories;
using DocForce.Domain.Common;
using DocForce.Domain.Errors;

namespace DocForce.Application.Features.Auth.Commands;

public record RevokeRefreshTokenCommand(
    string rawRefreshToken) : IRequest<Result>;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RevokeRefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshTokenRepository.GetByToken(request.rawRefreshToken);

        if (refreshToken == null)
            return Result.Failure(new NotFoundError("RefreshToken.NotFound", "This refresh token don't exist"));

        refreshToken.RevokeToken();
        await _unitOfWork.SaveEntitiesAsync();

        return Result.Success();
    }
}
