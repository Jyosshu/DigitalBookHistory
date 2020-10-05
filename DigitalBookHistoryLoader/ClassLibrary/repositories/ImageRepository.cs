using DigitalBookHistoryLoader.models;
using DigitalBookHistoryLoader.interfaces;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Linq;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DigitalBookHistoryLoader.repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ILogger<ImageRepository> _log;
        private readonly IConfiguration _config;

        public ImageRepository(ILogger<ImageRepository> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public List<TitleFields> GetTitleFields()
        {
            List<TitleFields> titleFields = new List<TitleFields>();

            try
            {
                string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
                using (IDbConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    titleFields = connection.Query<TitleFields>("SELECT Title, ArtKey FROM DigitalItem").ToList();
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
            List<string> existingImageRecords;
            string query = @"INSERT INTO Images (AltText, ArtKey, RemoteUrl) VALUES (@AltText, @ArtKey, @RemoteUrl)";

            try
            {
                existingImageRecords = GetExistingImageRecordsFromDb();

                string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
                using (IDbConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        foreach (ImageFields image in imageFields)
                        {
                            try
                            {
                                if (!existingImageRecords.Contains(image.ArtKey))
                                {
                                    var rowsInserted = connection.Execute(query, new { image.AltText, image.ArtKey, image.RemoteUrl }, transaction: transaction);

                                    if (rowsInserted != 1)
                                    {
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        existingImageRecords.Add(image.ArtKey);
                                    }
                                }
                            }
                            catch (DbException de)
                            {
                                _log.LogError(de.Message, de);
                                transaction.Rollback();
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

        private List<string> GetExistingImageRecordsFromDb()
        {
            string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
            using (IDbConnection connection = new SqlConnection(connstr))
            {
                connection.Open();

                try
                {
                    var results = connection.Query<string>("SELECT ArtKey FROM Images").ToList();
                    return results;
                }
                catch (DbException e)
                {
                    _log.LogError(e.Message, e);
                    return null;
                }
            }
        }
    }
}
