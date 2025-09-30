using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using BlocoDeNotasPWA;
using BlocoDeNotasPWA.Services;
using BlocoDeNotasPWA.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();

// auth + provider
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// app services
builder.Services.AddScoped<SupabaseService>();
builder.Services.AddScoped<NotaViewModel>();

var host = builder.Build();

// inicializar AuthService antes da UI (restaura token-refresh se necessário)
var auth = host.Services.GetRequiredService<AuthService>();
await auth.InitializeAsync();

await host.RunAsync();
