using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VibeSync.Api.Controllers.Base;
using VibeSync.Application.Contracts.Authentication;
using VibeSync.Application.Contracts.Services;
using VibeSync.Application.Exceptions;
using VibeSync.Application.Requests;
using VibeSync.Application.Responses;
using VibeSync.Application.UseCases;
using VibeSync.Infrastructure.Context;

namespace VibeSync.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthTokenService _tokenService;
    private readonly ILogger<AuthController> _logger;
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly IEmailSender _emailSender;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthTokenService tokenService,
        RegisterUserUseCase registerUserUseCase,
        IEmailSender emailSender) : base(logger)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _registerUserUseCase = registerUserUseCase;
        _emailSender = emailSender;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Email}", request.Email);
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new ErrorResponse("User not found", StatusCodes.Status401Unauthorized));

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new ErrorResponse("Invalid password", StatusCodes.Status401Unauthorized));

        var accessToken = _tokenService.GenerateAccessToken(new(user.Id, user.UserName!, user.UserName!));
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        });
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest payload)
    {
        try
        {
            await Handle(() => _registerUserUseCase.Execute(payload));

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
                return BadRequest(new ErrorResponse("User not found", StatusCodes.Status400BadRequest));

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth",
                new { userId = user.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", $"Clique aqui para confirmar seu e-mail no VibeSync: {confirmationLink}");

            return Ok("Registration successful. Please confirm your email.");
        }
        catch (UserAlreadyExistsException ex)
        {
            _logger.LogError(ex, "User already exists.");
            return StatusCode(409, new ErrorResponse("User already exists", StatusCodes.Status409Conflict, ex.ToString()));
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation error occurred while registering user.");
            return BadRequest(new ErrorResponse("Validation error", StatusCodes.Status400BadRequest, ex.ToString()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering user.");
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("An unexpected error occurred."));
        }
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == refreshToken &&
            u.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (user == null)
            return Unauthorized(new ErrorResponse("Refresh token inválido ou expirado", StatusCodes.Status401Unauthorized));

        var newAccessToken = _tokenService.GenerateAccessToken(new(user.Id, user.UserName!, user.UserName!));
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("Invalid user");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? Ok("Email confirmed!") : BadRequest("Email confirmation failed.");
    }
}