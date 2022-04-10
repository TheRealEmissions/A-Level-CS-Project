using Microsoft.Data.Sqlite;
using System;
using The_Project.Accounts;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    internal sealed class Messages : MustConstructWith<SqliteConnection>, ISqlTable
    {
        private readonly SqliteConnection _sqliteConnection;

        public Messages(SqliteConnection sqliteConnection) : base(sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
            //CreateTable();
        }

        public struct Schema
        {
            public string UserAccountId { get; }
            public string RefAccountId { get; }
            public DateTime Timestamp { get; }
            public string Message { get; }
            public bool Received { get; }

            public Schema(string userAccountId, string refAccountId, int timestamp, string message, bool received)
            {
                UserAccountId = userAccountId;
                RefAccountId = refAccountId;
                Timestamp = new DateTime(timestamp);
                Message = message;
                Received = received;
            }
        }

        public void CreateTable()
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS $database (
                    user_account_id TEXT NOT NULL,
                    recipient_account_id TEXT NOT NULL,
                    timestamp INTEGER NOT NULL,
                    message TEXT,
                    received BOOL NOT NULL,
                    FOREIGN KEY (user_account_id)
                        REFERENCES useraccounts (account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE,
                    FOREIGN KEY (recipient_account_id)
                        REFERENCES recipientaccounts (account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE
                )
            ";
            //Command.CommandType = System.Data.CommandType.Text;
            sqliteCommand.CommandText =
                sqliteCommand.CommandText.Replace("$database", _sqliteConnection.Database + ".messages");
            //Command.Parameters.AddWithValue("$database", _sqliteConnection.Database + ".messages");
            sqliteCommand.ExecuteNonQuery();
        }


        internal bool CreateMessageEntry(string accountId, string refAccountId, DateTime timestamp, string message, bool received)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText =
                @"INSERT INTO messages (user_account_id, recipient_account_id, timestamp, message, received) VALUES ($ACCOUNTID, $RECIPIENTID, $TIMESTAMP, $MESSAGE, $RECEIVED)";
            _ = sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            _ = sqliteCommand.Parameters.AddWithValue("$RECIPIENTID", refAccountId);
            _ = sqliteCommand.Parameters.AddWithValue("$TIMESTAMP",
                (int)((DateTimeOffset)DateTime.SpecifyKind(timestamp, DateTimeKind.Local)).ToUnixTimeSeconds());
            _ = sqliteCommand.Parameters.AddWithValue("$MESSAGE", message);
            _ = sqliteCommand.Parameters.AddWithValue("$RECEIVED", received);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }
    }
}