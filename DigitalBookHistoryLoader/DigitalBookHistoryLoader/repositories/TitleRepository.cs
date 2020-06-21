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

namespace DigitalBookHistoryLoader.repositories
{
    public class TitleRepository : ITitleRepository
    {
        private static HttpClient HttpClient = new HttpClient();
        private readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private IDbConnection OpenConnection()
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

        public bool LoadDigitalItemsToDb(List<DigitalItem> digitalItems, TaskLog taskLog)
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

                    //int nextArtistId = 0;

                    //int artistResult = connection.Execute("SELECT MAX(artistId) FROM artist");


                    //if (artistResult >= 0) nextArtistId = artistResult + 1;

                    artKeyUniqueCheck = GetExistingDigitalItemRows(taskLog); // TODO: Remove and check every for every digitalItem
                    artistUniqueCheck = GetExistingArtistRows(taskLog);

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (DigitalItem item in digitalItems)
                            {
                                // TODO: query database to see if digital_item, borrow and artist exists
                                //var titleExists = connection.Query("SELECT titleId FROM digital_item WHERE titleId = @TitleId", new { item.TitleId }).First();

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

                                    artKeyUniqueCheck.Add(item.TitleId, item.ArtKey);
                                }

                                // TODO: artist and digital_item should be a 1:many relationship (at least as Hoopla's history is concerned.  Their detailed view shows all authors and artists involved.

                                if (!artistUniqueCheck.ContainsValue(item.ArtistName))
                                {
                                    int insertedRowId = 0;

                                    insertedRowId = connection.Execute("INSERT INTO artist (artistName) OUTPUT artistId VALUES (@ArtistName)", new { item.ArtistName }, transaction: transaction);
                                    if (insertedRowId < 1)
                                    {
                                        transaction.Rollback();
                                    }

                                    // TODO: Change nextArtistId to insertedRowId after testing INSERT OUTPUT statement
                                    artistUniqueCheck.Add(insertedRowId, item.ArtistName);
                                    //nextArtistId++;
                                }


                                // TODO: Query to validate it doesn't exist?
                                var barrowExist = connection.Query("SELECT * FROM borrows WHERE titleId = @TitleId", new { item.TitleId }, transaction);

                                if (!barrowExist.Contains(item.Borrowed))
                                {
                                    var insertedRow = connection.Execute("INSERT INTO borrows (titleId, borrowed, returned) VALUES(@Borrowed, @Returned)", new { item.TitleId, item.Borrowed, item.Returned }, transaction);
                                    if (insertedRow != 1)
                                    {
                                        transaction.Rollback();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (DbException e)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                            taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
                        }
                    }
                    return true;

                }
            }
            catch (DbException e)
            {
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
                taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
                return false;
            }
            finally
            {
                taskLog.Close();
            }
        }

        private Dictionary<long, string> GetExistingDigitalItemRows(TaskLog taskLog)
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
                    taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
                }
            }

            return keyValuePairs;
        }

        private Dictionary<int, string> GetExistingArtistRows(TaskLog taskLog)
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
                    taskLog.AppendLine($"Exception Caught!  Message : {e.Message}");
                }
            }

            return keyValuePairs;
        }
    }
}
