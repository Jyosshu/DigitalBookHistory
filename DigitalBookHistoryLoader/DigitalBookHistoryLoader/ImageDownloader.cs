using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
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

        //public Task DownloadImageAsync(List<ImageFields> imageFieldsList)
        //{
        //    int successfulImages = 0;
        //    int failedImages = 0;
        //    List<ImageFields> failedImageList = new List<ImageFields>();


        //    return $"";
        //}
    }
}

