using Supabase;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(...);

		// Carregar das variáveis de ambiente ou secure storage
		var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");
		var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY");

		var options = new Supabase.SupabaseOptions
		{
			AutoConnectRealtime = false
		};

		var supabaseClient = new Client(supabaseUrl, supabaseKey, options);
		// não chame InitializeAsync aqui; faremos no serviço ou no startup async
		builder.Services.AddSingleton(supabaseClient);

		// registrar serviços e viewmodels
		builder.Services.AddSingleton<ISupabaseService, SupabaseService>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<RecordsViewModel>();

		return builder.Build();
	}
}
