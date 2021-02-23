using System;
using System.IO;
using System.Collections.Generic;
using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace DigitalBookHistoryLoader
{
    public class LoadDigitalBooks : ILoadDigitalBooks
    {
        private readonly IConfiguration _config;
        private readonly ILogger<LoadDigitalBooks> _log;
        private ITitleRepository _titleRepository;
        private readonly IImageRepository _imageRepository;

        private List<Borrow> _borrowsInDb;
        public bool loadSuccess = false;

        public LoadDigitalBooks(IConfiguration config, ILogger<LoadDigitalBooks> log, ITitleRepository titleRepository, IImageRepository imageRepository)
        {
            _config = config;
            _log = log;
            _titleRepository = titleRepository;
            _imageRepository = imageRepository;
        }

        public void Run(string filePath)
        {
            try
            {
                List<DigitalItem> digitalItems;
                string jsonToRead = ReadJsonToString(filePath);
                digitalItems = DeserializeDigitalItemsFromJson(jsonToRead);

                if (digitalItems.Count > 0)
                {
                    loadSuccess = LoadDigitalItemsToDb(digitalItems);

                    if (loadSuccess)
                    {
                        LoadImageFieldsToDb();
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message, e);
            }
        }

        private bool LoadDigitalItemsToDb(List<DigitalItem> digitalItems)
        {
            _borrowsInDb = _titleRepository.GetExistingBorrows();
            List<string> artistUniqueCheck = _titleRepository.GetExistingArtistRows();

            try
            {
                foreach (DigitalItem item in digitalItems)
                {
                    // TODO: artist and digital_item should be a 1:many relationship (at least as Hoopla's history is concerned.  Their detailed view shows all authors and artists involved.
                    Borrow borrow = new Borrow { TitleId = item.TitleId, Borrowed = item.Borrowed, Returned = item.Returned };
                    Tuple<bool, bool> borrowExists = BorrowExists(borrow);

                    if (borrowExists.Item1)
                    {
                        if (borrowExists.Item2)
                            continue;
                        else
                        {
                            _titleRepository.LoadBorrowToDb(borrow);
                        }
                    }
                    else
                    {
                        _titleRepository.LoadDigitalItemToDb(item);
                        _titleRepository.LoadBorrowToDb(borrow);
                    }

                    if (!artistUniqueCheck.Contains(item.ArtistName))
                    {
                        bool isSuccessful = _titleRepository.LoadArtistToDb(item.ArtistName);
                        if (isSuccessful)
                        {
                            artistUniqueCheck.Add(item.ArtistName);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message, e);
                return false;
            }
        }

        private void LoadImageFieldsToDb()
        {
            List<TitleFields> titleFieldsList;
            List<ImageFields> imageFieldsList = new List<ImageFields>();
            string remoteUrlBase = _config["RemoteImageUrl"];

            titleFieldsList = _imageRepository.GetTitleFields();

            foreach (TitleFields title in titleFieldsList)
            {
                ImageFields image = new ImageFields
                {
                    ArtKey = title.ArtKey,
                    AltText = title.Title
                };

                image.RemoteUrl = $"{remoteUrlBase}{title.ArtKey}_270.jpeg";

                imageFieldsList.Add(image);
            }

            bool imagesInserted = _imageRepository.CreateImageFields(imageFieldsList);

            if (imagesInserted == true)
            {
                _log.LogInformation("Image records inserted into the database");
            }
        }

        private List<DigitalItem> DeserializeDigitalItemsFromJson(string jsonToRead)
        {
            List<DigitalItem> digitalItems;

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            digitalItems = JsonSerializer.Deserialize<List<DigitalItem>>(jsonToRead, options);

            // Reorder the List to be sorted by date
            digitalItems.Reverse();

            return digitalItems;
        }

        private string ReadJsonToString(string fileToRead)
        {
            string results = null;

            try
            {
                using (var reader = new StreamReader(fileToRead))
                {
                    results = reader.ReadToEnd();
                }
            }
            catch (IOException e)
            {
                _log.LogError(e.Message, e);
            }

            return results;
        }

        private Tuple<bool, bool> BorrowExists(Borrow borrowToCheck)
        {
            bool bTitleExists = false;
            bool bBorrowExists = false;

            if (_borrowsInDb.Count > 0)
            {
                foreach (Borrow b in _borrowsInDb)
                {
                    if (borrowToCheck.TitleId == b.TitleId)
                    {
                        bTitleExists = true;

                        if (bTitleExists && b.Borrowed == borrowToCheck.Borrowed)
                            bBorrowExists = true;
                    }
                }
            }

            return new Tuple<bool, bool>(bTitleExists, bBorrowExists);
        }
    }
}
