using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

#nullable enable
namespace The_Project.Accounts
{
    public class AccountHandler
    {
        protected SqliteConnection? SQLConnection = null;
        public AccountHandler(string Username, string Password)
        {
            SQLConnection = StartSQLConnection();
            
        }

        public AccountHandler(string Username, string Password, string ConfPassword)
        {
            SQLConnection = StartSQLConnection();
        }

        private SqliteConnection StartSQLConnection()
        {
            SqliteConnection Connection = new("Data Source=account.db;Mode=ReadWriteCreate");
            Connection.Open();
            return Connection;
        }
    }
}
