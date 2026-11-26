using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Application.DTOs.Auth;
using DocForce.Application.Interfaces;
using DocForce.Application.Interfaces.Repositories;
using DocForce.Application.Interfaces.Services;
using DocForce.Domain.Common;
using DocForce.Domain.Entities;
using DocForce.Domain.Errors;

namespace DocForce.Application.Features.Auth.Commands;

public record LoginUserCommand(
    string Username,
    string Password) : IRequest<Result<TokenResponse>>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepoository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepoository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<Result<TokenResponse>> Handle(
        LoginUserCommand request, 
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepoository.GetByUsername(request.Username);

        if (user == null)
            return Result.Failure<TokenResponse>(new NotFoundError("User.NotFound", $"This user not found by this {request.Username} username"));

        var isUserPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        if (!isUserPassword)
            return Result.Failure<TokenResponse>(new BadRequestError("User.BadRequest", $"Incorrect username or password"));

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new TokenResponse(accessToken, refreshToken.Token);
    }
}
