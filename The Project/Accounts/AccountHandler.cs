using Microsoft.Data.Sqlite;

#nullable enable

namespace The_Project.Accounts
{
    public class AccountHandler
    {
        public readonly bool hasAccount = false;
        public readonly string AccountID = "NONE";

        public AccountHandler(string Username, string Password, SqliteConnection? Connection)
        {
            if (Connection is null) return;
            SqliteCommand? Command = Connection.CreateCommand();
            Command.CommandText = @"
                SELECT *
                FROM accounts
                WHERE username = $USERNAME
                AND password = $PASSWORD
            ";
            Command.Parameters.AddWithValue("$USERNAME", Username);
            Command.Parameters.AddWithValue("$PASSWORD", Password);

            SqliteDataReader? Reader = Command.ExecuteReader();
            if (Reader.Read())
            {
                hasAccount = true;
            }
        }

        public AccountHandler(string Username, string Password, string ConfPassword, SqliteConnection? Connection)
        {
            if (Connection is null) return;
        }
    }
}