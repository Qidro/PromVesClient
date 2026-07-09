using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromVesClient.Service;
using PromVesClient.Service.AppInfoService;
using PromVesClient.Service.UserService;
using Serilog;

namespace PromVesClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            var services = new ServiceCollection();
            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger);
            });

    //        var connectionString =
    //"Host=localhost;Port=5432;Database=PromVesDb;Username=postgres;Password=6767669";

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    "Host=localhost;Port=5432;Database=PromVesDb;Username=postgres;Password=6767669");
            });

            services.AddTransient<Form1>();
            services.AddTransient<MainMenu>();
            services.AddTransient<StaticWeighing>();

            //регистрация сервисов
            services.AddScoped<UserService>();
            services.AddScoped<HashPasswordService>();
            services.AddScoped<AppInfoService>();
            //регистрация одного экземпляра, чтобы все формы работали именно с ним
            services.AddSingleton<CurrentUserService>();

            var provider = services.BuildServiceProvider();
            ApplicationConfiguration.Initialize();
            Application.Run(
    provider.GetRequiredService<Form1>());
        }
    }
}