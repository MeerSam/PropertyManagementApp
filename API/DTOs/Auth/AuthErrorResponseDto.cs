using System;

namespace API.DTOs.Auth;

public class AuthErrorResponseDto
{
    public required string Message { get; init; }
    public List<string>? Errors { get; init; }
}
