using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace SqliteTest.Services
{
    public class DbService
    {
        #region singleton
        private static DbService _instance = new DbService();
        public static DbService Instance
        {
            get { return _instance; }
        }

        private DbService()
        {
        }
        #endregion

        private static readonly string _dbFilePath = @"d:\test\456.sqlite";
        private readonly object _dbLocker = new object();
        private readonly string _tableVersionName = "table_version";
        private static readonly string _connString = "data source=" + _dbFilePath;

        public void Init()
        {
            lock (_dbLocker)
            {
                CreateDbDirIfNotExists(_dbFilePath);
                CreateTableVersionIfNotExists();
            }
        }


        private void CreateDbDirIfNotExists(string dbFilePath)
        {
            if (File.Exists(dbFilePath)) return;

            string dir = Path.GetDirectoryName(_dbFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private void CreateTableVersionIfNotExists()
        {
            lock (_dbLocker)
            {
                string sql = string.Format("CREATE TABLE IF NOT EXISTS {0}(", _tableVersionName) +
                   "table_name varchar(100) PRIMARY KEY," +
                   "version INTEGER)";

                using (var conn = new SQLiteConnection(_connString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public int GetTableVersion(string tableName)
        {
            lock (_dbLocker)
            {
                using (var conn = new SQLiteConnection(_connString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = string.Format("SELECT version FROM {0} WHERE table_name = @tableName", _tableVersionName);
                        cmd.Parameters.AddWithValue("tableName", tableName);
                        var value = cmd.ExecuteScalar();

                        return Convert.ToInt32(value);
                    }
                }
            }
        }
    }
}
