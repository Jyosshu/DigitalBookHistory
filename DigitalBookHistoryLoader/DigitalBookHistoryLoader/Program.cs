using System;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.repositories;
using DigitalBookHistoryLoader.models;
using System.Collections.Generic;

namespace DigitalBookHistoryLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            IRemoteTitleRepository remoteTitleRepository = new RemoteTitleRepository();
            bool argsSucces = false;
            string fileToRead = null;

            while (argsSucces == false)
            {
                if (args.Length == 1 && args[0].Length > 0)
                {
                    argsSucces = true;
                    fileToRead = args[0];
                }
            }

            List<DigitalItem> digitalItems = remoteTitleRepository.GetHooplaHistory(fileToRead);

            remoteTitleRepository.LoadDigitalItemsToDb(digitalItems);
        }
    }
}
