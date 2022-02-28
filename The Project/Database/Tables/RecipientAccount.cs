using Microsoft.Data.Sqlite;
using The_Project.Accounts;
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
                CREATE TABLE IF NOT EXISTS $database (
                    nickname TEXT,
                    account_id TEXT NOT NULL,
                    ref_account_id TEXT NOT NULL UNIQUE,
                    FOREIGN KEY (ref_account_id)
                        REFERENCES useraccounts (account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE
                )
            ";
            Command.CommandText = Command.CommandText.Replace("$database", Connection.Database + ".recipientaccounts");
            //Command.Parameters.AddWithValue("$database", Connection.Database + ".recipientaccounts");
            Command.ExecuteNonQuery();
        }

        public bool CreateAccountEntry(string Nickname, string AccountId, string RefAccountId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"INSERT INTO recipientaccounts (nickname, account_id, ref_account_id) VALUES ($NICKNAME, $ACCOUNTID, $REFACCOUNTID)";
            Command.Parameters.AddWithValue("$NICKNAME", Nickname);
            Command.Parameters.AddWithValue("$ACCOUNTID", AccountId);
            Command.Parameters.AddWithValue("$REFACCOUNTID", RefAccountId);
            int Rows = Command.ExecuteNonQuery();
            return Rows > 0;
        }

        public bool CreateAccountEntry(string AccountId, string RefAccountId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"INSERT INTO recipientaccounts (account_id, ref_account_id) VALUES ($ACCOUNTID, $REFACCOUNTID)";
            Command.Parameters.AddWithValue("$ACCOUNTID", AccountId);
            Command.Parameters.AddWithValue("$REFACCOUNTID", RefAccountId);
            int Rows = Command.ExecuteNonQuery();
            return Rows > 0;
        }

        public Schema? GetAccountEntry(string Nickname, UserId RefUserId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                SELECT *
                FROM recipientaccounts
                WHERE nickname = $NICKNAME AND ref_account_id = $REFACCOUNTID
            ";
            Command.Parameters.AddWithValue("$NICKNAME", Nickname);
            Command.Parameters.AddWithValue("$REFACCOUNTID", RefUserId.AccountId);

            SqliteDataReader Reader = Command.ExecuteReader();

            if (!Reader.HasRows)
            {
                return null;
            }

            if (Reader.Read())
            {
                object[] RowColumns = new object[3];
                Reader.GetValues(RowColumns);

                Schema Schema = new(Nickname: RowColumns[0].ToString(), AccountId: RowColumns[1].ToString(), RefAccountId: RowColumns[2].ToString());
                return Schema;
            }

            return null;
        }

        public Schema? GetAccountEntry(UserId UserId, UserId RefUserId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                SELECT *
                FROM recipientaccounts
                WHERE account_id = $ACCOUNTID AND ref_account_id = $REFACCOUNTID
            ";
            Command.Parameters.AddWithValue("$ACCOUNTID", UserId.AccountId);
            Command.Parameters.AddWithValue("$REFACCOUNTID", RefUserId.AccountId);

            SqliteDataReader Reader = Command.ExecuteReader();

            if (!Reader.HasRows)
            {
                return null;
            }

            if (Reader.Read())
            {
                object[] RowColumns = new object[3];
                Reader.GetValues(RowColumns);

                Schema Schema = new(Nickname: RowColumns[0].ToString(), AccountId: RowColumns[1].ToString(), RefAccountId: RowColumns[2].ToString());
                return Schema;
            }

            return null;
        }

        public void UpdateNickname(string Nickname, UserId UserId, UserId RefUserId)
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                UPDATE recipientaccounts
                SET nickname = $NICKNAME
                WHERE account_id = $ACCOUNTID AND ref_account_id = $REFACCOUNTID
            ";
            Command.Parameters.AddWithValue("$NICKNAME", Nickname);
            Command.Parameters.AddWithValue("$ACCOUNTID", UserId.AccountId);
            Command.Parameters.AddWithValue("$REFACCOUNTID", RefUserId.AccountId);

            Command.ExecuteNonQuery();
            return;
        }
    }
}