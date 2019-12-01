using System;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;
using DigitalBookHistoryLoader.models;
using System.Collections.Generic;
using System.IO;
//using Utilities;

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool argsSuccess = false;
            string saveLocation = null;
            string fileToRead = null;
            string remoteUrlBase = "https://d2snwnmzyr8jue.cloudfront.net/";
            List<TitleFields> titleFieldsList;
            List<ImageFields> imageFieldsList = new List<ImageFields>();
            IImageRepository imageRepository = new ImageRepository();
            ITitleRepository titleRepository = new TitleRepository();
                        

            while (argsSuccess == false)
            {
                if (args.Length < 2)
                {
                    if (args.Length == 1 && args[0].ToLower() == "q")
                    {
                        Console.WriteLine("Exiting program.");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("This program reads a Hoopla history json file.  Loads the json objects into a database.  Uses those records to download images from Hoopla's image server," +
                            " and finally creates and loads image records in the database.");
                        Console.WriteLine($"Usage: {Environment.NewLine} \t\"jsonFileToRead\" \"saveLocationForImages\"");
                        Console.WriteLine($"Example: {Environment.NewLine} \t\"C:\\folder\\fileToRead.json\"\"C:\\folder\\folder\\\"");
                        Console.WriteLine("Type q and press Enter to exit program");
                        args[0] = Console.ReadLine();
                    }
                }
                else
                {
                    argsSuccess = true;
                    fileToRead = args[0];
                    saveLocation = args[1];
                }
            }

            Console.WriteLine($"The current base URL for the remote image server == {remoteUrlBase}.  {Environment.NewLine}Is this still correct? (y / n)");
            string choice = Console.ReadLine();

            // User enter new remote image hosting URL
            if (choice.ToLower() == "n" || choice.ToLower() == "no")
            {
                bool leaveLoop = false;
                Uri testUri = null;

                while (leaveLoop == false)
                {
                    Console.WriteLine("Enter the new temporary base URL, or q to exit program.");
                    string tempURL = Console.ReadLine();

                    if (tempURL.Length == 1 && (tempURL == "q" || tempURL == "Q" || tempURL.ToLower() == "quit"))
                    {
                        Environment.Exit(0);
                    }
                    else if (Uri.TryCreate(tempURL, UriKind.Absolute, out testUri))
                    {
                        Console.WriteLine($"Remote URL will temporarily be: {testUri}");
                        remoteUrlBase = tempURL;
                        leaveLoop = true;
                    }
                    else
                    {
                        Console.WriteLine($"You entered {tempURL}.  The format of this URL is invalid.{Environment.NewLine}The acceptable format is: http://www.example.com");
                    }
                }

            }

            List<DigitalItem> digitalItems;

            try
            {
                if (!File.Exists(fileToRead))
                {
                    throw new FileNotFoundException($"{fileToRead} does not exist.");
                }
                else
                {
                    digitalItems = titleRepository.GetHooplaHistory(fileToRead);

                    titleRepository.LoadDigitalItemsToDb(digitalItems);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"There was an error accessing {fileToRead}.  {ex.Message}. {Environment.NewLine}Press q or x to exit.");
                var input = Console.ReadKey();

                if (input.ToString().ToUpper() == "Q" || input.ToString().ToUpper() == "X")
                {
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an exception.  {ex.Message}");
            }

            if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);

            titleFieldsList = imageRepository.GetTitleFields();

            foreach (TitleFields title in titleFieldsList)
            {
                ImageFields image = new ImageFields
                {
                    ArtKey = title.ArtKey,
                    AltText = title.Title
                };

                image.RemoteUrl = $"{remoteUrlBase}{title.ArtKey}_270.jpeg";
                image.LocalPath = $"{saveLocation}\\{title.ArtKey}_270.jpeg";

                imageFieldsList.Add(image);
            }

            ImageDownloader.DownloadImage(imageFieldsList);

            bool imagesInserted = imageRepository.CreateImageFields(imageFieldsList);

            if (imagesInserted == true)
            {
                Console.WriteLine("Image records inserted into the database");
            }
        }
    }
}
