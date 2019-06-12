using System;
using System.Collections.Generic;
using System.Net;
using DigitalBookHistoryLoader.models;

namespace DigitalBookHistoryLoader
{
    public class ImageDownloader
    {

        public static void DownloadImage(List<ImageFields> imageFieldsList)
        {
            using (var client = new WebClient())
            {
                foreach (ImageFields image in imageFieldsList)
                    try
                    {
                        client.DownloadFile(new Uri(image.RemoteUrl), image.LocalPath);                       
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine($"{Environment.NewLine} Exception Caught!");
                        Console.WriteLine($"Message : {e.Message}");
                    }
            }
        }


    }
}

