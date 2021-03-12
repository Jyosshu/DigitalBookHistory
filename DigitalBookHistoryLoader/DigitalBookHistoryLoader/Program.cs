using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;
using System.Threading.Tasks;

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<ITitleRepository, TitleRepository>();
                    services.AddTransient<IImageRepository, ImageRepository>();
                })
                .UseSerilog()
                .Build();

            try
            {
                var httpSvc = ActivatorUtilities.CreateInstance<HooplaHttp>(host.Services);
                string jsonResponse = await httpSvc.GetHooplaHistory();

                var svc = ActivatorUtilities.CreateInstance<LoadDigitalBooks>(host.Services);
                svc.Run(jsonResponse);
            }
            catch (IOException ie)
            {
                Log.Error(ie.Message, ie);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}
