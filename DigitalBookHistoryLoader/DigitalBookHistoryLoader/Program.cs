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

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool argsSuccess = false;
            string fileToRead = null;

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

            while (argsSuccess == false)
            {
                if (args.Length < 1)
                {
                    if (args.Length == 0 && args[0].ToLower() == "q")
                    {
                        Log.Information("Exiting program.");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("This program reads a Hoopla history json file.  Loads the json objects into a database.  Uses those records to download images from Hoopla's image server," +
                            " and finally creates and loads image records in the database.");
                        Console.WriteLine($"Usage: {Environment.NewLine} \t\"jsonFileToRead\"");
                        Console.WriteLine($"Example: {Environment.NewLine} \t\"C:\\folder\\fileToRead.json\"");
                        Console.WriteLine("Type q and press Enter to exit program");
                        args[0] = Console.ReadLine();
                    }
                }
                else
                {
                    argsSuccess = true;
                    fileToRead = args[0];
                }
            }

            try
            {
                if (!File.Exists(fileToRead))
                {
                    throw new FileNotFoundException($"{ fileToRead } does not exist.");
                }
                else
                {
                    var svc = ActivatorUtilities.CreateInstance<LoadDigitalBooks>(host.Services);
                    svc.Run(fileToRead);

                }
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex.Message, ex);
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
