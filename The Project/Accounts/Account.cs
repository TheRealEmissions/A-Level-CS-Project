using Microsoft.Data.Sqlite;

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