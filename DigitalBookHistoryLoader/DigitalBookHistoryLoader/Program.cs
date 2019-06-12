using System;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;
using DigitalBookHistoryLoader.models;
using System.Collections.Generic;
using System.IO;

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            bool argsSuccess = false;
            string saveLocation = null;
            string fileToRead = null;
            List<TitleFields> titleFieldsList = new List<TitleFields>();
            List<ImageFields> imageFieldsList = new List<ImageFields>();
            IImageRepository imageRepository = new ImageRepository();
            ITitleRepository titleRepository = new TitleRepository();
                        

            while (argsSuccess == false)
            {
                if (args.Length < 2)
                {
                    if (args.Length == 1 && args[0] == "q")
                    {
                        Console.WriteLine("Exiting program.");
                        Environment.Exit(0);
                    }
                    else if (args.Length == 1 && args[0] == "q")
                    {
                        Console.WriteLine("Exiting program.");
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine($"Usage: {Environment.NewLine} \t\"fileToRead\" \"saveLocation\"");
                        Console.WriteLine($"Example: {Environment.NewLine} \t\"C:\\folder\\fileToRead.json\"\"C:\\folder\\folder\\\"");
                        Console.WriteLine("Type q and press Enter to exit program");
                    }
                }
                else
                {
                    argsSuccess = true;
                    fileToRead = args[0];
                    saveLocation = args[1];
                }
            }


            List<DigitalItem> digitalItems = titleRepository.GetHooplaHistory(fileToRead);

            titleRepository.LoadDigitalItemsToDb(digitalItems);


            if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);

            titleFieldsList = imageRepository.GetTitleFields();

            foreach (TitleFields title in titleFieldsList)
            {
                ImageFields image = new ImageFields
                {
                    ArtKey = title.ArtKey,
                    AltText = title.Title
                };

                image.RemoteUrl = $"https://d2snwnmzyr8jue.cloudfront.net/{title.ArtKey}_270.jpeg";
                image.LocalPath = $"{saveLocation}\\{title.ArtKey}_270.jpeg";

                imageFieldsList.Add(image);
            }

            ImageDownloader.DownloadImage(imageFieldsList);

            bool imagesInserted = imageRepository.CreateImageFields(imageFieldsList);
        }
    }
}
