using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Data;
using Microsoft.Data.SqlClient;
using Npgsql;
using System.Runtime.InteropServices;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.models;
using Dapper;
using System.Linq;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalBookHistoryLoader.repositories
{
    public class TitleRepository : ITitleRepository
    {
        private static HttpClient HttpClient = new HttpClient();
        private readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private List<Borrow> borrowsInDb;
        private readonly ILogger<TitleRepository> _log;
        private readonly AppSettings _appSettings;

        public TitleRepository(ILogger<TitleRepository> log, IOptionsSnapshot<AppSettings> appSettings)
        {
            _log = log;
            _appSettings = appSettings.Value;
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection connection;

            if (isWindows == true)
            {
                connection = new SqlConnection(_appSettings.ConnectionStrings.DigitalBookSQL);
            }
            else
            {
                connection = new NpgsqlConnection(_appSettings.ConnectionStrings.DigitalBookPostgres);
            }

            connection.Open();

            return connection;
        }

        private List<Borrow> GetExistingBorrows()
        {
            List<Borrow> results = null;

            try
            {
                using (var connection = OpenConnection())
                {
                    string borrowSelectQuery = ""; // TODO: Create query that returns each titleID, borrowed and returned date.

                    results = connection.Query<Borrow>(borrowSelectQuery).ToList();
                }
            }
            catch (SqlException se)
            {
                _log.LogError(se.Message, se);
            }

            return results;
        }

        private Tuple<bool, bool> BorrowExists(Borrow borrowToCheck)
        {
            bool bTitleExists = false;
            bool bBorrowExists = false;

            if (borrowsInDb.Count > 0)
            {
                foreach (Borrow b in borrowsInDb)
                {
                    if (borrowToCheck.TitleId == b.TitleId)
                    {
                        bTitleExists = true;

                        if (bTitleExists && b.Borrowed == borrowToCheck.Borrowed)
                            bBorrowExists = true;
                    }
                }
            }

            return new Tuple<bool, bool>(bTitleExists, bBorrowExists);
        }

        public bool LoadDigitalItemsToDb(List<DigitalItem> digitalItems)
        {
            try
            {
                using (var connection = OpenConnection())
                {

                    Dictionary<long, string> artKeyUniqueCheck = new Dictionary<long, string>();
                    Dictionary<int, string> artistUniqueCheck = new Dictionary<int, string>();

                    string query = @"INSERT INTO digital_item (titleId, title, kindId, artistName, demo, pa, edited, artKey, circId, fixedLayout, readAlong) "; // borrowed, returned, 
                    string values = @"VALUES (@TitleId, @Title, @KindId, @ArtistName, @Demo, @Pa, @Edited, @ArtKey, @CircId, @FixedLayout, @ReadAlong);"; //@Borrowed, @Returned, 

                    // TODO: change id to be Identity

                    int nextArtistId = 0;

                    int artistResult = connection.Execute("SELECT MAX(artistId) FROM artist");


                    if (artistResult >= 0) nextArtistId = artistResult + 1;

                    artKeyUniqueCheck = GetExistingDigitalItemRows(); // TODO: Remove and check every for every digitalItem
                    artistUniqueCheck = GetExistingArtistRows();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (DigitalItem item in digitalItems)
                            {
                                // TODO: query database to see if digital_item, borrow and artist exists
                                // TODO: artist and digital_item should be a 1:many relationship (at least as Hoopla's history is concerned.  Their detailed view shows all authors and artists involved.
                                Tuple<bool, bool> borrowExists = BorrowExists(new Borrow { TitleId = item.TitleId, Borrowed = item.Borrowed, Returned = item.Returned });

                                if (borrowExists.Item1)
                                {
                                    if (borrowExists.Item2)
                                        continue;
                                    else
                                    {
                                        // TODO: Load borrow for existing titleID
                                    }
                                }
                                else
                                {
                                    // TODO: Load title and borrow for new item
                                }

                                // TODO: refactor the unique artist check
                                if (!artistUniqueCheck.ContainsValue(item.ArtistName))
                                {
                                    int insertedRowId = connection.Execute("INSERT INTO artist (artistId, artistName) OUTPUT artistId VALUES (@ArtistId, @ArtistName)", new { ArtistId = nextArtistId, item.ArtistName }, transaction: transaction);
                                    if (insertedRowId != 1)
                                    {
                                        transaction.Rollback();
                                    }

                                    // TODO: Change nextArtistId to insertedRowId after testing INSERT OUTPUT statement
                                    artistUniqueCheck.Add(nextArtistId, item.ArtistName);
                                    nextArtistId++;
                                }

                                //var titleExists = connection.Query("SELECT titleId FROM digital_item WHERE titleId = @ItemTitleId").First();

                                if (!artKeyUniqueCheck.ContainsKey(item.TitleId))
                                {
                                    var insertedRow = connection.Execute(query + values,
                                        new
                                        {
                                            item.TitleId,
                                            item.Title,
                                            item.KindId,
                                            item.ArtistName,
                                            Demo = (item.Demo == false) ? 0 : 1,
                                            PA = (item.PA == false) ? 0 : 1,
                                            Edited = (item.Edited == false) ? 0 : 1,
                                            item.ArtKey,
                                            item.CircId,
                                            FixedLayout = (item.FixedLayout == false) ? 0 : 1,
                                            ReadAlong = (item.ReadAlong == false) ? 0 : 1
                                        }, transaction);

                                    if (insertedRow != 1)
                                    {
                                        transaction.Rollback();
                                    }

                                    // TODO: Query to validate it doesn't exist?
                                    insertedRow = connection.Execute("INSERT INTO borrows (titleId, borrowed, returned) VALUES(@Borrowed, @Returned)", new { item.Borrowed, item.Returned }, transaction);
                                    if (insertedRow != 1)
                                    {
                                        transaction.Rollback();
                                    }

                                    artKeyUniqueCheck.Add(item.TitleId, item.ArtKey);
                                }
                            }

                            transaction.Commit();
                        }
                        catch (DbException e)
                        {
                            transaction.Rollback();
                            _log.LogError(e.Message, e);
                        }
                    }
                    return true;

                }
            }
            catch (DbException e)
            {
                _log.LogError(e.Message, e);
                return false;
            }
        }

        private Dictionary<long, string> GetExistingDigitalItemRows()
        {
            Dictionary<long, string> keyValuePairs = new Dictionary<long, string>();

            using (var connection = OpenConnection())
            {
                try
                {
                    var results = connection.Query("SELECT titleId, artKey FROM digital_item").ToDictionary(row => (long)row.titleId, row => (string)row.artKey);
                    keyValuePairs = results;
                }
                catch (DbException e)
                {
                    _log.LogError(e.Message, e);
                }
            }

            return keyValuePairs;
        }

        private Dictionary<int, string> GetExistingArtistRows()
        {
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

            using (var connection = OpenConnection())
            {
                try
                {
                    var results = connection.Query("SELECT artistId, artistName FROM artist").ToDictionary(row => (int)row.artistId, row => (string)row.artistName);
                    keyValuePairs = results;
                }
                catch (DbException e)
                {
                    _log.LogError(e.Message, e);
                }
            }

            return keyValuePairs;
        }
    }
}
