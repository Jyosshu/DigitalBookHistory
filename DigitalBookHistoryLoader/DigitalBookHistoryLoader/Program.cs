using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;
using CommandLine;
using CommandLine.Text;

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Options parsedOptions = new Options();

            var options = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(parsed => parsedOptions = parsed)
                .WithNotParsed(errors => Log.Error("There was an error parsing the command line arguments.", errors));

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
                if (!File.Exists(parsedOptions.InputFile))
                {
                    throw new FileNotFoundException($"{ parsedOptions.InputFile } does not exist.");
                }
                else
                {
                    var svc = ActivatorUtilities.CreateInstance<LoadDigitalBooks>(host.Services);
                    svc.Run(parsedOptions.InputFile);

                }
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
