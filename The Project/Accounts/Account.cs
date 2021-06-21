using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Accounts
{
    public class Account : AccountHandler
    {
        public string Username = "";
        public Account(string Username, string Password, SqliteConnection Connection) : base(Username, Password, Connection)
        {
            this.Username = Username;
        }

        public Account(string Username, string Password, string ConfPassword, SqliteConnection Connection) : base(Username, Password, ConfPassword, Connection)
        {
            this.Username = Username;
        }
    }
}
