using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using DigitalBookHistoryAPI.Models;
using DigitalBookHistoryAPI.Settings;
using DigitalBookHistoryAPI.Interface;

namespace DigitalBookHistoryAPI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppSettings _appSettings;

        public BookRepository(IOptionsSnapshot<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public List<DigitalBook> GetBooks()
        {
            List<DigitalBook> bookList;

            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {                
                bookList = connection.Query<DigitalBook, Image, Kind, DigitalBook>(getBooksQuery + " ORDER BY di.id", 
                    map: (digitalBook, image, kind) =>
                    {
                        digitalBook.Image = image;
                        digitalBook.Kind = kind;
                        return digitalBook;
                    }
                    ).AsList();
            }

            return bookList;
        }

        public DigitalBook GetBookById(int bookId)
        {
            DigitalBook book;

            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                var result = connection.Query<DigitalBook, Image, Kind, DigitalBook>($"{getBooksQuery} WHERE di.id = @bookId",
                    map: (digitalBook, image, kind) =>
                    {
                        digitalBook.Image = image;
                        digitalBook.Kind = kind;
                        return digitalBook;
                    }, new { BookId = bookId }).AsList();

                book = result.Count > 0 ? result.FirstOrDefault() : null;
            }

            return book;
        }

        public List<DigitalBook> GetBooksByAuthor(string authorName)
        {
            // TODO: clean user input string
            List<DigitalBook> results;
            int inputLength = authorName.Length;

            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                results = connection.Query<DigitalBook, Image, Kind, DigitalBook>($"{getBooksQuery} WHERE LEFT(di.artistName, @InputLength) = @ArtistName ORDER BY di.id",
                    map: (digitalBook, image, kind) =>
                    {
                        digitalBook.Image = image;
                        digitalBook.Kind = kind;
                        return digitalBook;
                    }, new { artistName = authorName, InputLength = inputLength }).AsList();
            }

            return results;
        }

        private static readonly string getBooksQuery = @"SELECT di.id
, di.titleId
, di.title
, di.kindId
, di.artistName
, di.artKey
, di.borrowed
, di.returned
, i.id
, i.altText
, i.artKey
, i.remoteUrl
, i.localUrl
, ki.id
, ki.name
, ki.singular
, ki.plural
FROM digital_item di
INNER JOIN images i ON i.artKey = di.artKey
INNER JOIN kind ki ON ki.id = di.kindId";
    }
}
