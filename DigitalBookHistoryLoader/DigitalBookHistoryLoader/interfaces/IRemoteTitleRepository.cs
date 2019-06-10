using System;
using System.Collections.Generic;
using DigitalBookHistoryLoader.models;

namespace DigitalBookHistoryLoader.interfaces
{
    public interface IRemoteTitleRepository
    {
        List<DigitalItem> GetHooplaHistory(string fileToRead);
        bool LoadDigitalItemsToDb(List<DigitalItem> digitalItems);
    }
}
