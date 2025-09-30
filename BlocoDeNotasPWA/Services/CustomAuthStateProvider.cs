using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlocoDeNotasPWA.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly AuthService _auth;

    public CustomAuthStateProvider(AuthService auth)
    {
        _auth = auth;
        _auth.AuthStateChanged += NotifyAuthStateChanged;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        if (_auth.IsAuthenticated)
        {
            var claims = new List<Claim>();
            var userId = _auth.GetUserId();
            var email = _auth.GetUserEmail();

            if (!string.IsNullOrEmpty(userId))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            if (!string.IsNullOrEmpty(email))
                claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim(ClaimTypes.Name, email ?? userId ?? "user"));

            identity = new ClaimsIdentity(claims, "supabase");
        }

        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    private void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
