using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;

namespace DigitalBookHistoryLoader
{
    public class LoadDigitalBooks
    {
        //string _fileToRead;
        ITitleRepository titleRepository = new TitleRepository();

        //public LoadDigitalBooks(string fileToRead)
        //{
        //    _fileToRead = fileToRead;
        //}

        public List<DigitalItem> BuildDigitalItemsFromJson(string jsonToRead)
        {
            List<DigitalItem> digitalItems = new List<DigitalItem>();

            try
            {
                string results = GetHooplaHistoryJson(jsonToRead);
                digitalItems = JsonConvert.DeserializeObject<List<DigitalItem>>(results);
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
            }

            digitalItems.Reverse();

            return digitalItems;
        }

        private string GetHooplaHistoryJson(string fileToRead)
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
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
            }

            return results;
        }
    }
}
