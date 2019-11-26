using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                string query = "";
                bookList = connection.Query<DigitalBook>(query).AsList();
            }

            return bookList;
        }

        public DigitalBook GetBook(int Id)
        {
            DigitalBook book;

            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                string query = "";
                book = connection.QueryFirstOrDefault(query);
            }

            return book;
        }
    }
}
