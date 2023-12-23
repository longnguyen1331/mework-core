using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using WebClient.RequestHttp;

namespace WebClient.Identity
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider 
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NotificationService _notificationService;


    public ApiAuthenticationStateProvider(ILocalStorageService localStorage,
            NotificationService notificationService
        )
    {
        _localStorage = localStorage;
        _notificationService = notificationService;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var savedToken = "";
        try
        {
            InjectServiceForHttpClient();
            savedToken = await _localStorage.GetItemAsync<string>("my-access-token"); 
            
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            RequestClient.AttachToken(savedToken);

            var claims = ParseClaimsFromJwt1(savedToken);

            if (!CheckExpiredToken(claims))
            {
                await _localStorage.RemoveItemAsync("my-access-token");
                await _localStorage.RemoveItemAsync("my-refresh-token");

                _notificationService.Notify(new NotificationMessage
                {

                    Severity = NotificationSeverity.Warning,
                    Summary =
                        "Phiên làm việc của bạn đã hết.Hệ thống sẽ tự logout sau 4s nữa",
                    Detail = "", Duration = 4000
                });
                Thread.Sleep(4000);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
          

            
            
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        catch (Exception e)
        {
        }
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

    }

    public bool CheckExpiredToken(IEnumerable<Claim> claims)
    {
        var expiredClaim =  claims.FirstOrDefault(x => x.Type == "exp");
        var epochTime = long.Parse(expiredClaim.Value);
        DateTime tokenTime = DateTime.UnixEpoch.AddSeconds(epochTime);
        if (tokenTime.ToLocalTime() < DateTime.Now)
        {
            return false;
        }

        return true;
    }
    
    
    private void InjectServiceForHttpClient()
    {
        RequestClient.InjectServices(_localStorage);
    }

    public void MarkUserAsAuthenticated(string userName)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName) }, "apiauth"));
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        await _localStorage.RemoveItemAsync("my-access-token");
        await _localStorage.RemoveItemAsync("my-refresh-token");
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        
        NotifyAuthenticationStateChanged(authState);
    }

    
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
    
        if (roles != null)
        {
            if (roles.ToString().Trim().StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                foreach (var parsedRole in parsedRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                }
            }
            else 
            {
                claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }
        
        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

        return claims;
    }
    
    private IEnumerable<Claim> ParseClaimsFromJwt1(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();

        var decodedValue = handler.ReadJwtToken(jwt);
        return decodedValue.Claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
    }
}