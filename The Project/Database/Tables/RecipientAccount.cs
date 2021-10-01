using Microsoft.Data.Sqlite;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public class RecipientAccount : MustConstructWith<SqliteConnection>, ISQLTable
    {
        private readonly SqliteConnection Connection;

        public RecipientAccount(SqliteConnection connection) : base(connection)
        {
            Connection = connection;
            //CreateTable();
        }

        public struct Schema
        {
            public string Nickname { get; }
            public string AccountId { get; }
            public string RefAccountId { get; }

            public Schema(string Nickname, string AccountId, string RefAccountId)
            {
                this.Nickname = Nickname;
                this.AccountId = AccountId;
                this.RefAccountId = RefAccountId;
            }
        }

        public void CreateTable()
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                CREATE TABLE IF NOT EXISTS db.recipientaccounts (
                    nickname TEXT NOT NULL,
                    account_id TEXT NOT NULL,
                    ref_account_id TEXT NOT NULL UNIQUE,
                    FOREIGN KEY (ref_account_id)
                        REFERENCES useraccounts (account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE
                )
            ";
            Command.ExecuteNonQuery();
        }

        public Schema CreateAccountEntry(string Nickname, string AccountId, string RefAccountId)
        {
            SqliteCommand Command = new();
            Command.CommandText = @"";
            Command.ExecuteNonQuery();
        }
    }
}