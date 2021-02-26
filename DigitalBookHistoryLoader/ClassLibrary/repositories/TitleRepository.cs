using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Data;
using Microsoft.Data.SqlClient;
using DigitalBookHistoryLoader.interfaces;
using DigitalBookHistoryLoader.models;
using Dapper;
using System.Linq;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace DigitalBookHistoryLoader.repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly ILogger<TitleRepository> _log;
        private readonly IConfiguration _config;

        public TitleRepository(ILogger<TitleRepository> log, IConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public List<Borrow> GetExistingBorrows()
        {
            List<Borrow> results = null;

            try
            {
                string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
                using (IDbConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    string borrowSelectQuery = "SELECT di.TitleId, b.Borrowed, b.Returned FROM DigitalItem di Inner Join Borrows b ON b.TitleId = di.TitleId";

                    results = connection.Query<Borrow>(borrowSelectQuery).ToList();
                }
            }
            catch (SqlException se)
            {
                _log.LogError(se.Message, se);
            }

            return results;
        }

        public void LoadDigitalItemToDb(DigitalItem digitalItem)
        {
            string query = @"INSERT INTO DigitalItem (TitleId, Title, KindId, ArtistName, Demo, Pa, Edited, ArtKey, CircId, FixedLayout, ReadAlong) ";
            string values = @"VALUES (@TitleId, @Title, @KindId, @ArtistName, @Demo, @Pa, @Edited, @ArtKey, @CircId, @FixedLayout, @ReadAlong);";

            string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
            using (IDbConnection connection = new SqlConnection(connstr))
            {
                connection.Open();

                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insertedRow = connection.Execute(query + values,
                            new
                            {
                                digitalItem.TitleId,
                                digitalItem.Title,
                                digitalItem.KindId,
                                digitalItem.ArtistName,
                                Demo = (digitalItem.Demo == false) ? 0 : 1,
                                PA = (digitalItem.PA == false) ? 0 : 1,
                                Edited = (digitalItem.Edited == false) ? 0 : 1,
                                digitalItem.ArtKey,
                                digitalItem.CircId,
                                FixedLayout = (digitalItem.FixedLayout == false) ? 0 : 1,
                                ReadAlong = (digitalItem.ReadAlong == false) ? 0 : 1
                            }, transaction);

                        if (insertedRow != 1)
                        {
                            transaction.Rollback();
                        }

                        transaction.Commit();
                    }
                    catch (DbException de)
                    {
                        _log.LogError(de.Message, de);
                        transaction.Rollback();
                    }
                }
            }
        }

        public void LoadBorrowToDb(Borrow borrow)
        {
            try
            {
                string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
                using (IDbConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var insertedRow = connection.Execute("INSERT INTO Borrows (TitleId, Borrowed, Returned) VALUES (@TitleId, @Borrowed, @Returned)", borrow, transaction);

                            if (insertedRow != 1)
                            {
                                transaction.Rollback();
                            }

                            transaction.Commit();
                        }
                        catch (DbException de)
                        {
                            transaction.Rollback();
                            _log.LogError(de.Message, de);
                        }
                    }
                }
            }
            catch (DbException de)
            {
                _log.LogError(de.Message, de);
            }
        }

        public bool LoadArtistToDb(string artistName)
        {
            try
            {
                string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
                using (IDbConnection connection = new SqlConnection(connstr))
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int insertedRow = connection.Execute("INSERT INTO Artist (ArtistName) VALUES (@ArtistName)", new { artistName }, transaction: transaction);
                            if (insertedRow != 1)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (DbException de)
                        {
                            _log.LogError(de.Message, de);
                            transaction.Rollback();
                        }

                        transaction.Commit();
                    }
                }

                return true;
            }
            catch (DbException de)
            {
                _log.LogError(de.Message, de);
                return false;
            }
        }

        private Dictionary<long, string> GetExistingDigitalItemRows()
        {
            Dictionary<long, string> keyValuePairs = new Dictionary<long, string>();

            string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
            using (IDbConnection connection = new SqlConnection(connstr))
            {
                connection.Open();

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

        public List<string> GetExistingArtistsFromDb()
        {
            string connstr = _config.GetConnectionString("SQLCONNSTR_DIGITALBOOK");
            using (IDbConnection connection = new SqlConnection(connstr))
            {
                connection.Open();

                try
                {
                    var results = connection.Query<string>("SELECT ArtistName FROM Artist").ToList();
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
