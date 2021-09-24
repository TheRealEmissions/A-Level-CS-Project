using Microsoft.Data.Sqlite;
using The_Project.Database;

#nullable enable

namespace The_Project.Accounts
{
    public class AccountHandler : UserAccount
    {
        public bool HasAccount { get; }
        public string AccountID { get; } = "NONE";

        public AccountHandler(string Username, string Password, SqliteConnection? Connection)
        {
            if (Connection is null)
            {
                return;
            }

            SqliteCommand? Command = Connection.CreateCommand();
            Command.CommandText = @"
                SELECT *
                FROM accounts
                WHERE username = $USERNAME
                ";
            Command.Parameters.AddWithValue("$USERNAME", Username);

            SqliteDataReader? Reader = Command.ExecuteReader();
            if (Reader.Read())
            {
                HasAccount = true;
            }
        }

        public AccountHandler(string Username, string Password, string ConfPassword, SqliteConnection? Connection)
        {
            if (Connection is null)
            {
                return;
            }

            if (Password != ConfPassword)
            {
                return;
            }
        }

        public AccountHandler(string Username)
        {

        }
    }
}