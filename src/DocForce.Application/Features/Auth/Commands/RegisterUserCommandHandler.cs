using DocForce.Application.Common.Dispatcher;
using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Application.DTOs.Auth;
using DocForce.Application.Interfaces;
using DocForce.Application.Interfaces.Repositories;
using DocForce.Application.Interfaces.Services;
using DocForce.Domain.Common;
using DocForce.Domain.Entities;
using DocForce.Domain.Errors;

namespace DocForce.Application.Features.Auth.Commands;

public record RegisterUserCommand(
    string Username,
    string Password,
    string ConfirmPassword) : IRequest<Result<TokenResponse>>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<TokenResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async ValueTask<Result<TokenResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.ExistByUsername(request.Username))
        {
            return Result.Failure<TokenResponse>(new ConflictError("User.Conflict", "Username is already in use"));
        }

        var user = new User(request.Username, _passwordHasher.HashPassword(request.Password));

        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(user.Id);

        await _userRepository.AddAsync(user, cancellationToken);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveEntitiesAsync(cancellationToken);

        return new TokenResponse(accessToken, refreshToken.Token);
    }
}
