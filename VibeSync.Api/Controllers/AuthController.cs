using System.ComponentModel.DataAnnotations;
using System.Web;
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
    [ProducesResponseType(typeof(UserRegisteredResponse), StatusCodes.Status201Created)] // 201 para criação de recurso
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest payload)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponse("Invalid payload.", StatusCodes.Status400BadRequest, ModelState.ToString()));

        try
        {
            UserResponse user = await _registerUserUseCase.Execute(payload);

            if (user == null)
            {
                _logger.LogError("IRegisterUserUseCase returned null for email: {Email}", payload.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("User registration failed unexpectedly."));
            }

            var userEntity = await _userManager.FindByEmailAsync(user.Email);

            if (userEntity == null)
            {
                _logger.LogError("User not found after registration: {Email}", payload.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("User registration failed unexpectedly."));
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userEntity);
            await _emailSender.SendConfirmationEmailAsync(userEntity.AsUser(), token);

            var response = new UserRegisteredResponse(userEntity.Id, userEntity.Email!, "Registration successful. Please check your email to confirm your account.");


            return StatusCode(StatusCodes.Status201Created, response);
        }
        catch (UserAlreadyExistsException ex)
        {
            _logger.LogWarning(ex, "Attempt to register existing user: {Email}", payload.Email);
            return StatusCode(StatusCodes.Status409Conflict, new ErrorResponse("User already exists.", StatusCodes.Status409Conflict, ex.Message));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error during registration for: {Email}", payload.Email);
            return BadRequest(new ErrorResponse("Validation error.", StatusCodes.Status400BadRequest, ex.ToString() ?? ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while registering user: {Email}", payload.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse("An unexpected error occurred. Please try again later."));
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

    [HttpPost("confirm-email")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest payload)
    {
        var user = await _userManager.FindByIdAsync(payload.UserId);
        if (user == null) return BadRequest(new ErrorResponse("Usuário inválido", StatusCodes.Status400BadRequest));

        var token = payload.Token.Replace(" ", "+");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? Ok(new { message = "Email confirmed!" }) : BadRequest(new ErrorResponse("Email confirmation failed.", StatusCodes.Status400BadRequest));
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ErrorResponse("Email is required.", StatusCodes.Status400BadRequest));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Security best practice: do not reveal if user exists or email is confirmed
            return Ok(new { message = "If the email is registered and confirmed, a password reset link will be sent." });
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        // Map ApplicationUser to Domain.Domains.User for email sender
        var domainUser = user.AsUser();
        await _emailSender.SendPasswordResetEmailAsync(domainUser, resetToken);
        return Ok(new { message = "If the email is registered and confirmed, a password reset link will be sent." });
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest(new ErrorResponse("All fields are required.", StatusCodes.Status400BadRequest));

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest(new ErrorResponse("Invalid email.", StatusCodes.Status400BadRequest));

        var decodedToken = HttpUtility.UrlDecode(request.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        if (result.Succeeded)
            return Ok(new { message = "Password has been reset successfully." });

        var error = result.Errors.FirstOrDefault()?.Description ?? "Password reset failed.";
        return BadRequest(new ErrorResponse(error, StatusCodes.Status400BadRequest));
    }
}