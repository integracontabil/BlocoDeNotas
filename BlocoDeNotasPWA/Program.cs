using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using BlocoDeNotasPWA;
using BlocoDeNotasPWA.Services;
using BlocoDeNotasPWA.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBlazoredLocalStorage();

// 🔹 SupabaseService registrado com DI
builder.Services.AddScoped<SupabaseService>();

// ViewModel
builder.Services.AddScoped<NotaViewModel>();

await builder.Build().RunAsync();
