using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GoodHamburger.Blazor;
using GoodHamburger.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API base URL — em produção viria de configuração
builder.Services.AddScoped(_ =>
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5000/")
    };
    client.DefaultRequestHeaders.Add("X-API-Key", "goodhamburger-local-dev-key");
    return client;
});

builder.Services.AddScoped<IApiService, ApiService>();

await builder.Build().RunAsync();
