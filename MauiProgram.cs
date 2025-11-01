using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DKSKMaui.Backend.Data;
using DKSKMaui.Backend.Services;
using Radzen;

namespace DKSKMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		try
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

			// Configure SQLite DbContext for MAUI with better error handling
			try
			{
				var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
				System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");
				
				builder.Services.AddDbContext<AppDbContext>(options =>
					options.UseSqlite($"Data Source={dbPath}")
						   .EnableSensitiveDataLogging()
						   .LogTo(message => System.Diagnostics.Debug.WriteLine(message)));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Database configuration error: {ex.Message}");
				throw;
			}

			// Register Radzen services with error handling
			try
			{
				builder.Services.AddScoped<DialogService>();
				builder.Services.AddScoped<NotificationService>();
				builder.Services.AddScoped<TooltipService>();
				builder.Services.AddScoped<ContextMenuService>();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Radzen services registration error: {ex.Message}");
				throw;
			}

			// Register application services with error handling
			try
			{
				builder.Services.AddScoped<CompanyService>();
				builder.Services.AddScoped<PropertiesService>();
				builder.Services.AddScoped<ContractorService>();
				builder.Services.AddScoped<SupervisorService>();
				builder.Services.AddScoped<InvoiceService>();
				builder.Services.AddScoped<MyCompanyInfoService>();
				builder.Services.AddScoped<JobDescriptionService>();
				builder.Services.AddScoped<GlobalStateService>();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Application services registration error: {ex.Message}");
				throw;
			}

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

			var app = builder.Build();

			// Add global exception handler
			AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
			{
				var exception = (Exception)args.ExceptionObject;
				System.Diagnostics.Debug.WriteLine($"Unhandled exception: {exception}");
			};

			TaskScheduler.UnobservedTaskException += (sender, args) =>
			{
				System.Diagnostics.Debug.WriteLine($"Unobserved task exception: {args.Exception}");
				args.SetObserved();
			};

			// Ensure database is created with comprehensive error handling
			try
			{
				using (var scope = app.Services.CreateScope())
				{
					var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
					var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
					
					logger.LogInformation("Initializing database...");
					context.Database.EnsureCreated();
					logger.LogInformation("Database initialized successfully");
					
					// Test database connection
					var canConnect = context.Database.CanConnect();
					logger.LogInformation($"Database connection test: {canConnect}");
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex}");
				// Don't throw here - let the app start even if DB fails initially
			}

			return app;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Critical error in MauiProgram: {ex}");
			throw;
		}
	}
}
