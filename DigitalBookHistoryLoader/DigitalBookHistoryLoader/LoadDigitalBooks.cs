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
        private List<DigitalItem> _digitalItems;
        private TaskLog _taskLog;
        public bool loadSuccess = false;

        private ITitleRepository _titleRepo = new TitleRepository();

        public LoadDigitalBooks(TaskLog taskLog)
        {
            _taskLog = taskLog;
        }

        public void GetDigitalItemsFromString(string filename)
        {
            string jsonToRead = ReadJsonToString(filename);
            _digitalItems = DeserializeDigitalItemsFromJson(jsonToRead);
            

            if (_digitalItems.Count > 0)
            {
                loadSuccess = _titleRepo.LoadDigitalItemsToDb(_digitalItems, _taskLog);
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
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                _taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
            }

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
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                _taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
            }

            return results;
        }
    }
}
