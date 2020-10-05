using System;
using System.Collections.Generic;
using DigitalBookHistoryLoader.models;

namespace DigitalBookHistoryLoader.interfaces
{
    public interface ITitleRepository
    {
        List<string> GetExistingArtistRows();
        List<Borrow> GetExistingBorrows();
        void LoadDigitalItemToDb(DigitalItem digitalItem);
        void LoadBorrowToDb(Borrow borrow);
        bool LoadArtistToDb(string artistName);
    }
}
