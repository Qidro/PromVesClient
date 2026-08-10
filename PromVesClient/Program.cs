using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromVesClient.Service;
using PromVesClient.Service.AppInfoService;
using PromVesClient.Service.ReceiptsService;
using PromVesClient.Service.StaticWeighingService;
using PromVesClient.Service.TcpService;
using PromVesClient.Service.UserService;
using PromVesClient.Services;
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

            services.AddDbContextFactory<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(
                    "Host=localhost;Port=5432;Database=PromVesDb;Username=postgres;Password=6767669");
            });

            services.AddTransient<Form1>();
            services.AddTransient<MainMenu>();
            services.AddTransient<StaticWeighing>();
            services.AddTransient<frmWeighingReceipts>();
            services.AddTransient<ComPortSettingsForm>();
            services.AddTransient<UserManagementForm>();
            services.AddTransient<ChangeUserForm>();
            services.AddTransient<ReceiptPrintSettingsForm>();
            services.AddTransient<WagonForm>();

            //регистрация сервисов
            services.AddScoped<UserService>();
            services.AddScoped<HashPasswordService>();
            services.AddScoped<AppInfoService>();
            services.AddScoped<StaticWeighingService>();
            services.AddSingleton<ComPortService>();
            services.AddScoped<TcpService>();
            services.AddScoped<ReceiptsService>();
            services.AddScoped<ExcelReportService>();
            services.AddSingleton<ReceiptPrintSettingsService>();
            services.AddTransient<WagonService>();

            //регистрация одного экземпляра, чтобы все формы работали именно с ним
            services.AddSingleton<CurrentUserService>();

            var provider = services.BuildServiceProvider();
            ApplicationConfiguration.Initialize();
            Application.Run(
    provider.GetRequiredService<Form1>());
        }
    }
}