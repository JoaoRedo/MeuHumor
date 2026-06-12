using System.Security.Claims;

namespace MeuHumor.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(sub) || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAccessException("Token inválido ou usuário não identificado.");

        return userId;
    }
}
