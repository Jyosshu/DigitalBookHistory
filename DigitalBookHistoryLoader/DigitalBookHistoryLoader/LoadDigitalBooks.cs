using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using Microsoft.Extensions.Logging;

namespace DigitalBookHistoryLoader
{
    public class LoadDigitalBooks : ILoadDigitalBooks
    {
        private readonly ILogger<LoadDigitalBooks> _log;
        private ITitleRepository _titleRepository;
        public bool loadSuccess = false;

        public LoadDigitalBooks(ILogger<LoadDigitalBooks> log, ITitleRepository titleRepository)
        {
            _log = log;
            _titleRepository = titleRepository;
        }

        public void Run(string filePath)
        {
            List<DigitalItem> digitalItems;
            string jsonToRead = ReadJsonToString(filePath);
            digitalItems = DeserializeDigitalItemsFromJson(jsonToRead);

            if (digitalItems.Count > 0)
            {
                loadSuccess = _titleRepository.LoadDigitalItemsToDb(digitalItems);
            }
        }

        private List<DigitalItem> DeserializeDigitalItemsFromJson(string jsonToRead)
        {
            List<DigitalItem> digitalItems = new List<DigitalItem>();

            try
            {
                digitalItems = JsonConvert.DeserializeObject<List<DigitalItem>>(jsonToRead);
            }
            catch (AggregateException e)
            {
                _log.LogError(e.Message, e);
            }

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
    }
}
