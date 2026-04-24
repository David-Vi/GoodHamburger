using Microsoft.AspNetCore.Authentication;

namespace GoodHamburger.Api.Auth;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string HeaderName { get; set; } = ApiKeyAuthenticationDefaults.HeaderName;
    public string ApiKey { get; set; } = string.Empty;
}
