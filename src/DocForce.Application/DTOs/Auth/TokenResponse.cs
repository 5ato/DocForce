namespace DocForce.Application.DTOs.Auth;

public record TokenResponse(
    string AccessToken,
    string RefreshToken);
