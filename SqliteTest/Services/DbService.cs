using SqliteTest.Models;
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
        private static readonly string _connString = "data source=" + _dbFilePath;
        private readonly object _dbLocker = new object();
        private readonly string _tableVersionName = "table_version";


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
            lock (_dbLocker)
            {
                if (File.Exists(dbFilePath)) return;

                string dir = Path.GetDirectoryName(_dbFilePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
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

        public SqliteTemplateWrapper GetSqliteTemplateWrapper(SqlTemplate sqlTemplate)
        {
            SqliteTemplateWrapper result = new SqliteTemplateWrapper();
            result.SqlExpression = sqlTemplate.SqlExpression;
            List<SQLiteParameter> newParams = new List<SQLiteParameter>();
            result.Params = newParams;
            if (string.IsNullOrWhiteSpace(sqlTemplate.SqlExpression) || sqlTemplate.Params == null || sqlTemplate.Params.Length == 0)
            {
                return result;
            }

            string sqlExpression = sqlTemplate.SqlExpression;
            String[] sqlPieces = sqlExpression.Split('?');
            sqlExpression = string.Join("", sqlPieces.Select((d, i) => d + (i == sqlPieces.Length - 1 ? "" : "{" + i + "}")));

            for (int i = 0; i < sqlTemplate.Params.Length; i++)
            {
                object value = sqlTemplate.Params[i];
                string name = string.Format("p{0}", i);
                SQLiteParameter sQLiteParameter = new SQLiteParameter(name, value);
                newParams.Add(sQLiteParameter);
                sqlExpression = sqlExpression.Replace("{" + i + "}", "@" + name);
            }

            result.SqlExpression = sqlExpression;
            return result;
        }
    }
}
