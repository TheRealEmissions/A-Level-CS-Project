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
        public AccountHandler(string Username, string Password, SqliteConnection? Connection)
        {
            if (Connection is null) return;
        }

        public AccountHandler(string Username, string Password, string ConfPassword, SqliteConnection? Connection)
        {
            if (Connection is null) return;   
        }
    }
}
