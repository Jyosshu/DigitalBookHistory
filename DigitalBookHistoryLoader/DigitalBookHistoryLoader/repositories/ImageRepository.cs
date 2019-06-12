using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using Npgsql;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DigitalBookHistoryLoader.repositories
{
    public class ImageRepository : IImageRepository
    {
        private bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

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

        public List<TitleFields> GetTitleFields()
        {
            List<TitleFields> titleFields = new List<TitleFields>();

            try
            {
                using (var connection = OpenConnection())
                {
                    titleFields = connection.Query<TitleFields>("SELECT title, artKey FROM digital_item").AsList();
                }
            }
            catch (DbException e)
            {
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
            }

            return titleFields;
        }

        public bool CreateImageFields(List<ImageFields> imageFields)
        {
            Dictionary<int, string> existingImageRecords = new Dictionary<int, string>();
            int currentMaxImageId = 0;
            string query = @"INSERT INTO images (altText, artKey, remoteUrl, localUrl) VALUES (@AltText, @ArtKey, @RemoteUrl, @LocalUrl)";

            try
            {
                existingImageRecords = GetExistingImageRecordsFromDb();
                currentMaxImageId = existingImageRecords.Max().Key;

                using (var connection = OpenConnection())
                {
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (ImageFields image in imageFields)
                        {
                            if (!existingImageRecords.ContainsValue(image.ArtKey))
                            {
                                var rowsInserted = connection.Execute(query, new { image.AltText, image.ArtKey, image.RemoteUrl, LocalUrl = image.LocalPath }, transaction: transaction);

                                if (rowsInserted != 1)
                                {
                                    transaction.Rollback();
                                }
                                else
                                {
                                    existingImageRecords.Add(currentMaxImageId, image.ArtKey);
                                    currentMaxImageId++;
                                }
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (DbException e)
            {
                Console.WriteLine($"Exception Caught!  Message : {e.Message}");
            }

            return true;
        }

        private Dictionary<int, string> GetExistingImageRecordsFromDb()
        {
            Dictionary<int, string> keyValuePairs = new Dictionary<int, string>();

            using (var connection = OpenConnection())
            {
                try
                {
                    var results = connection.Query("SELECT id, artKey FROM image").ToDictionary(row => (int)row.id, row => (string)row.artKey);
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
