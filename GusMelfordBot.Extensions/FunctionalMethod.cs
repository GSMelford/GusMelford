using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using GusMelfordBot.Extensions.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GusMelfordBot.Extensions;

public static class FunctionalMethod
{
    public static T IfNullThrow<T>([NotNull] this T? value, Exception? exception = null)
    {
        if (value is null)
        {
            throw exception ?? new Exception();
        }

        return value;
    }
    
    public static Guid GetUserId(this HttpContext httpContext)
    {
        ClaimsPrincipal currentUser = httpContext.User;
        string? userIdString = currentUser.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        return currentUser.Claims.Any() && Guid.TryParse(userIdString, out Guid userId) 
            ? userId 
            : throw new UnauthorizedException("User not found");
    }
}