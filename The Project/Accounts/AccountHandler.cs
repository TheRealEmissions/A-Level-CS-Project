using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace The_Project.Accounts
{
    public class AccountHandler
    {
        private SqlConnection? SqlConnection = null;
        public AccountHandler(string Username, string Password)
        {
            SqlConnection = StartSQLConnection();
        }

        public AccountHandler(string Username, string Password, string ConfPassword)
        {
            SqlConnection = StartSQLConnection();
        }

        public SqlConnection StartSQLConnection()
        {
            string ConnectionString = "";
            SqlConnection connection = new(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
