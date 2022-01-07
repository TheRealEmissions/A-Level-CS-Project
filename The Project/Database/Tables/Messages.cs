using Microsoft.Data.Sqlite;
using System;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public class Messages : MustConstructWith<SqliteConnection>, ISQLTable
    {
        private readonly SqliteConnection Connection;

        public Messages(SqliteConnection Connection) : base(Connection)
        {
            this.Connection = Connection;
            //CreateTable();
        }

        public struct Schema
        {
            public string UserAccountId { get; }
            public string RefAccountId { get; }
            public DateTime Timestamp { get; }
            public string Message { get; }
            public bool Received { get; }

            public Schema(string UserAccountId, string RefAccountId, int Timestamp, string Message, bool Received)
            {
                this.UserAccountId = UserAccountId;
                this.RefAccountId = RefAccountId;
                this.Timestamp = new DateTime(Timestamp);
                this.Message = Message;
                this.Received = Received;
            }
        }

        public void CreateTable()
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
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
            Command.CommandText = Command.CommandText.Replace("$database", Connection.Database + ".messages");
            //Command.Parameters.AddWithValue("$database", Connection.Database + ".messages");
            Command.ExecuteNonQuery();
        }
    }
}