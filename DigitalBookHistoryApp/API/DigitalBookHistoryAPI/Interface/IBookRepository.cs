using System.Collections.Generic;
using DigitalBookHistoryAPI.Models;

namespace DigitalBookHistoryAPI.Interface
{
    interface IBookRepository
    {
        List<DigitalBook> GetBooks();
        DigitalBook GetBook(int bookId);
    }
}
