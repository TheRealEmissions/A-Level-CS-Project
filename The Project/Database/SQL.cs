using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace The_Project.Database
{
    public class SQL
    {
        public SqliteConnection? Connection = null;
        public SQL()
        {
            Connection = Start();
        }

        private SqliteConnection Start()
        {
            SqliteConnection Connection = new("Data Source=account.db;Mode=ReadWriteCreate");
            Connection.Open();
            return Connection;
        }
    }
}
