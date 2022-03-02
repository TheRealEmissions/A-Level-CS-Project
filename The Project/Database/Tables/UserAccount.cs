using Microsoft.Data.Sqlite;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

#nullable enable

namespace The_Project.Database.Tables
{
    public class UserAccount : MustConstructWith<SqliteConnection>, ISQLTable
    {
        private readonly SqliteConnection Connection;

        public UserAccount(SqliteConnection connection) : base(connection)
        {
            Connection = connection;
            //CreateTable();
        }

        public struct Schema
        {
            public string Username { get; }
            public string Password { get; }
            public string AccountId { get; }

            public Schema(string Username, string Password, string AccountId)
            {
                this.Username = Username;
                this.Password = Password;
                this.AccountId = AccountId;
            }
        }

        public void CreateTable()
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                CREATE TABLE IF NOT EXISTS $database (
                    username TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    account_id TEXT PRIMARY KEY
                )
            ";
            Command.CommandText = Command.CommandText.Replace("$database", Connection.Database + ".useraccounts");
            //Command.Parameters.AddWithValue("$database", Connection.Database + ".useraccounts");
            Command.ExecuteNonQuery();
        }

        public Schema? GetAccountEntry(string Username)
        {
            SqliteCommand? Command = Connection.CreateCommand();
            Command.CommandText = @"
                SELECT *
                FROM useraccounts
                WHERE username = $USERNAME
                ";
            _ = Command.Parameters.AddWithValue("$USERNAME", Username);

            SqliteDataReader? Reader = Command.ExecuteReader();

            if (!Reader.HasRows)
            {
                return null;
            }

            if (Reader.Read())
            {
                object[] RowColumns = new object[3];
                Reader.GetValues(RowColumns);

                Schema Schema = new(Username: RowColumns[0].ToString(), Password: RowColumns[1].ToString(), AccountId: RowColumns[2].ToString());
                return Schema;
            }
            return null;
        }

        public bool CreateAccountEntry(string Username, string PasswordHash, string AccountId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"INSERT INTO useraccounts (username, password, account_id) VALUES ($USERNAME, $PASSWORD, $ACCOUNTID)";
            _ = Command.Parameters.AddWithValue("$USERNAME", Username);
            _ = Command.Parameters.AddWithValue("$PASSWORD", PasswordHash);
            _ = Command.Parameters.AddWithValue("$ACCOUNTID", AccountId);
            int Rows = Command.ExecuteNonQuery();
            return Rows > 0;
        }

        public bool UpdatePasswordInEntry(string AccountId, string Password)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"UPDATE useraccounts SET password = $PASSWORD WHERE account_id = $ACCOUNTID";
            _ = Command.Parameters.AddWithValue("$PASSWORD", Password);
            _ = Command.Parameters.AddWithValue("$ACCOUNTID", AccountId);
            int Rows = Command.ExecuteNonQuery();
            return Rows > 0;
        }
    }
}