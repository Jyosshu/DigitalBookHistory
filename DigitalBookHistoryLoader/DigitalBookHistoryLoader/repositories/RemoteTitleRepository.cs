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
    public class RemoteTitleRepository : IRemoteTitleRepository
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
                    Dictionary<long, string> listCheck = new Dictionary<long, string>();
                    string query = @"INSERT INTO digital_item (id, titleId, title, kindId, artistName, demo, pa, edited, artKey, borrowed, returned, circId, fixedLayout, readAlong) ";
                    string values = @"VALUES (@Id, @TitleId, @Title, @KindId, @ArtistName, @Demo, @Pa, @Edited, @ArtKey, @Borrowed, @Returned, @CircId, @FixedLayout, @ReadAlong);";

                    int nextId = 0;

                    var result = connection.Execute("SELECT MAX(id) FROM digital_item");

                    if (result >= 0)
                    {
                        nextId = result + 1;
                    }

                    var results = connection.Query("SELECT titleId, artKey FROM digital_item").ToDictionary(row => (long)row.titleId, row => (string)row.artKey);

                    listCheck = results;

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (DigitalItem item in digitalItems)
                            {
                                if (!listCheck.ContainsKey(item.TitleId))
                                {
                                    var insertedRow = connection.Execute(query + values,
                                        new
                                        {
                                            Id = nextId,
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

                                    if (insertedRow != 1)
                                    {
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        nextId++;
                                    }

                                    if (!listCheck.ContainsKey(item.TitleId))
                                    {
                                        listCheck.Add(item.TitleId, item.ArtKey);
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
    }
}
