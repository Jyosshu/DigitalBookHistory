using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DigitalBookHistoryLoader
{
    public class ImageDownloader : IImageDownloader
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ImageDownloader> _log;
        private IImageRepository _imageRepository;

        public ImageDownloader(IConfiguration config, ILogger<ImageDownloader> log, IImageRepository imageRepository)
        {
            _config = config;
            _log = log;
            _imageRepository = imageRepository;
        }

        public void Run()
        {
            List<TitleFields> titleFieldsList;
            List<ImageFields> imageFieldsList = new List<ImageFields>();
            string saveLocation = _config.GetValue<string>("ImageSaveLocation");
            string remoteUrlBase = _config.GetValue<string>("RemoteImageUrl");

            if (!Directory.Exists(saveLocation)) Directory.CreateDirectory(saveLocation);

            titleFieldsList = _imageRepository.GetTitleFields();

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

            DownloadImage(imageFieldsList);

            bool imagesInserted = _imageRepository.CreateImageFields(imageFieldsList);

            if (imagesInserted == true)
            {
                _log.LogInformation("Image records inserted into the database");
            }
        }

        private void DownloadImage(List<ImageFields> imageFieldsList)
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
                        _log.LogError(e.Message, e);
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

