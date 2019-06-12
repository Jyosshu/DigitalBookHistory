using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using System.Runtime.InteropServices;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.models;
using Dapper;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Data.Common;

namespace DigitalBookHistoryLoader.repositories
{
    public class TitleRepository : ITitleRepository
    {
        private static HttpClient HttpClient = new HttpClient();
        private readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public IDbConnection OpenConnection()
        {
            IDbConnection connection;

            if (isWindows == true)
            {
                connection = new SqlConnection(AppSettings.ConnectionStrings.DefaultConnection);
            }
            else
            {
                connection = new NpgsqlConnection(AppSettings.ConnectionStrings.PostgresConnection);
            }

            connection.Open();

            return connection;
        }

        public List<DigitalItem> GetHooplaHistory(string fileToRead)
        {
            List<DigitalItem> digitalItems = new List<DigitalItem>();

            try
            {
                string results = GetHooplaHistoryJson(fileToRead);
                digitalItems = JsonConvert.DeserializeObject<List<DigitalItem>>(results);
            }
            catch (AggregateException e)
            {
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
            }

            digitalItems.Reverse();

            return digitalItems;
        }

        public bool LoadDigitalItemsToDb(List<DigitalItem> digitalItems)
        {
            using (var connection = OpenConnection())
            {
                try
                {
                    Dictionary<long, string> artKeyUniqueCheck = new Dictionary<long, string>();
                    Dictionary<int, string> artistUniqueCheck = new Dictionary<int, string>();

                    string query = @"INSERT INTO digital_item (id, titleId, title, kindId, artistName, demo, pa, edited, artKey, borrowed, returned, circId, fixedLayout, readAlong) ";
                    string values = @"VALUES (@Id, @TitleId, @Title, @KindId, @ArtistName, @Demo, @Pa, @Edited, @ArtKey, @Borrowed, @Returned, @CircId, @FixedLayout, @ReadAlong);";

                    int nextDigitalItemId = 0;
                    int nextArtistId = 0;

                    var result = connection.Execute("SELECT MAX(id) FROM digital_item");
                    var artistResult = connection.Execute("SELECT MAX(artistId) FROM artist");

                    if (result >= 0) nextDigitalItemId = result + 1;
                    if (artistResult >= 0) nextArtistId = artistResult + 1;

                    artKeyUniqueCheck = GetExistingDigitalItemRecordsFromDb();
                    artistUniqueCheck = GetExistingArtistRecordsFromDb();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (DigitalItem item in digitalItems)
                            {
                                if (!artistUniqueCheck.ContainsValue(item.ArtistName))
                                {
                                    var insertedRow = connection.Execute("INSERT INTO artist (artistId, artistName) VALUES (@ArtistId, @ArtistName)", new { ArtistId = nextArtistId, item.ArtistName }, transaction: transaction);
                                    if (insertedRow != 1) transaction.Rollback();
                                    else
                                    {
                                        artistUniqueCheck.Add(key: nextArtistId, value: item.ArtistName);
                                        nextArtistId++;
                                    }
                                }

                                if (!artKeyUniqueCheck.ContainsKey(item.TitleId))
                                {
                                    var insertedRow = connection.Execute(query + values,
                                        new
                                        {
                                            Id = nextDigitalItemId,
                                            item.TitleId,
                                            item.Title,
                                            item.KindId,
                                            item.ArtistName,
                                            Demo = (item.Demo == false) ? 0 : 1,
                                            PA = (item.PA == false) ? 0 : 1,
                                            Edited = (item.Edited == false) ? 0 : 1,
                                            item.ArtKey,
                                            item.Borrowed,
                                            item.Returned,
                                            item.CircId,
                                            FixedLayout = (item.FixedLayout == false) ? 0 : 1,
                                            ReadAlong = (item.ReadAlong == false) ? 0 : 1
                                        }, transaction);

                                    if (insertedRow != 1) transaction.Rollback();

                                    else nextDigitalItemId++;

                                    if (!artKeyUniqueCheck.ContainsKey(item.TitleId))
                                    {
                                        artKeyUniqueCheck.Add(item.TitleId, item.ArtKey);
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (DbException e)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                        }
                    }
                    return true;
                }
                catch (DbException e)
                {                    
                    Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                    return false;
                }            
            }
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

        private Dictionary<long, string> GetExistingDigitalItemRecordsFromDb()
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
                    Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                }
            }

            return keyValuePairs;
        }

        private Dictionary<int, string> GetExistingArtistRecordsFromDb()
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
                    Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                }
            }

            return keyValuePairs;
        }
    }
}
