using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using Npgsql;
using System.Linq;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DigitalBookHistoryLoader.repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ILogger<ImageRepository> _log;
        private readonly AppSettings _appSettings;
        private bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public ImageRepository(ILogger<ImageRepository> log, IOptionsSnapshot<AppSettings> appSettings)
        {
            _log = log;
            _appSettings = appSettings.Value;
        }

        public IDbConnection OpenConnection()
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
                _log.LogError(e.Message, e);
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
                _log.LogError(e.Message, e);
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
                    _log.LogError(e.Message, e);
                }
            }

            return keyValuePairs;
        }
    }
}
