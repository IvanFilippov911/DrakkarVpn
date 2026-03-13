using DrakkarVpn.Shared.Errors;
using DrakkarVpn.Shared.Errors.DomainErrors;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.HostInfrastructure.Infrastructure.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next    = next;
        _logger  = logger;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ctx, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        _logger.LogError(ex, "Unhandled exception");
        ctx.Response.ContentType = "application/json";

        switch (ex)
        {
            case DomainException de:
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code    = de.Code,
                    area    = de.Area.ToString(),
                    message = de.Message
                });
                break;
            
            case ValidationException vex:
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code   = "VALIDATION_FAILED",
                    errors = vex.Errors.Select(e => new
                    {
                        field   = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
                break;
            
            case UserBannedException:
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code    = "USER_BANNED",
                    message = ex.Message
                });
                break;
            
            case InvalidOperationException ioe when ioe.Message == "User not found":
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code    = "USER_NOT_FOUND",
                    message = ioe.Message
                });
                break;
            
            case PeersRevokeFailedException prf:
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code    = "PEERS_REVOKE_FAILED",
                    scope   = prf.Scope,  
                    scopeId = prf.ScopeId,
                    revoked = prf.Revoked,
                    failed  = prf.Failed,
                    message = prf.Message
                });
                break;
            
            default:
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(new
                {
                    code    = "INTERNAL_ERROR",
                    message = "Unexpected error"
                });
                break;
        }
    }
}

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}