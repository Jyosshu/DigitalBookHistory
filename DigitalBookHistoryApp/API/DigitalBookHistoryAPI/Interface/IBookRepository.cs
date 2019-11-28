using System.Collections.Generic;
using DigitalBookHistoryAPI.Models;

namespace DigitalBookHistoryAPI.Interface
{
    public interface IBookRepository
    {
        List<DigitalBook> GetBooks();
        DigitalBook GetBookById(int bookId);
    }
}
