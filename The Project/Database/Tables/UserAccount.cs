using Microsoft.Data.Sqlite;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

#nullable enable

namespace The_Project.Database.Tables
{
    public class UserAccount : MustConstructWith<SqliteConnection>, ISqlTable
    {
        private readonly SqliteConnection _sqliteConnection;

        public UserAccount(SqliteConnection sqliteConnection) : base(sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
            //CreateTable();
        }

        public struct Schema
        {
            public string Username { get; }
            public string Password { get; }
            public string AccountId { get; }

            public Schema(string username, string password, string accountId)
            {
                this.Username = username;
                this.Password = password;
                this.AccountId = accountId;
            }
        }

        public void CreateTable()
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS $database (
                    username TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    account_id TEXT PRIMARY KEY
                )
            ";
            sqliteCommand.CommandText = sqliteCommand.CommandText.Replace("$database", _sqliteConnection.Database + ".useraccounts");
            //Command.Parameters.AddWithValue("$database", _sqliteConnection.Database + ".useraccounts");
            sqliteCommand.ExecuteNonQuery();
        }

        public Schema? GetAccountEntry(string username)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                SELECT *
                FROM useraccounts
                WHERE username = $USERNAME
                ";
            _ = sqliteCommand.Parameters.AddWithValue("$USERNAME", username);

            SqliteDataReader dataReader = sqliteCommand.ExecuteReader();

            if (!dataReader.HasRows)
            {
                return null;
            }

            if (dataReader.Read())
            {
                object[] rowColumns = new object[3];
                dataReader.GetValues(rowColumns);

                Schema schema = new(username: rowColumns[0].ToString(), password: rowColumns[1].ToString(), accountId: rowColumns[2].ToString());
                return schema;
            }
            return null;
        }

        public bool CreateAccountEntry(string username, string passwordHash, string accountId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"INSERT INTO useraccounts (username, password, account_id) VALUES ($USERNAME, $PASSWORD, $ACCOUNTID)";
            _ = sqliteCommand.Parameters.AddWithValue("$USERNAME", username);
            _ = sqliteCommand.Parameters.AddWithValue("$PASSWORD", passwordHash);
            _ = sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }

        public bool UpdatePasswordInEntry(string accountId, string password)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"UPDATE useraccounts SET password = $PASSWORD WHERE account_id = $ACCOUNTID";
            _ = sqliteCommand.Parameters.AddWithValue("$PASSWORD", password);
            _ = sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }
    }
}