using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace SqliteTest.Models
{
    public class SqliteTemplateWrapper
    {
        public string SqlExpression { get; set; }
        public IEnumerable<SQLiteParameter> Params { get; set; }
    }
}
