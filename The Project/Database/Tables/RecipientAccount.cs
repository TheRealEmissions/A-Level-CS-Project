using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public sealed class RecipientAccount : MustConstructWith<SqliteConnection>, ISqlTable
    {
        private readonly SqliteConnection _sqliteConnection;

        internal RecipientAccount(SqliteConnection sqliteConnection) : base(sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
            //CreateTable();
        }

        public struct Schema
        {
            private string Nickname { get; }
            private string AccountId { get; }
            private string RefAccountId { get; }

            internal Schema(string nickname, string accountId, string refAccountId)
            {
                Nickname = nickname;
                AccountId = accountId;
                RefAccountId = refAccountId;
            }
        }

        public void CreateTable()
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
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
            sqliteCommand.CommandText =
                sqliteCommand.CommandText.Replace("$database", _sqliteConnection.Database + ".recipientaccounts");
            //Command.Parameters.AddWithValue("$database", _sqliteConnection.Database + ".recipientaccounts");
            sqliteCommand.ExecuteNonQuery();
        }

        internal bool CreateAccountEntry(string nickname, string accountId, string refAccountId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText =
                @"INSERT INTO recipientaccounts (nickname, account_id, ref_account_id) VALUES ($NICKNAME, $ACCOUNTID, $REFACCOUNTID)";
            sqliteCommand.Parameters.AddWithValue("$NICKNAME", nickname);
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refAccountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }

        internal bool CreateAccountEntry(string accountId, string refAccountId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText =
                @"INSERT INTO recipientaccounts (account_id, ref_account_id) VALUES ($ACCOUNTID, $REFACCOUNTID)";
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refAccountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }

        internal Schema? GetAccountEntry(string nickname, UserId refUserId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                SELECT *
                FROM recipientaccounts
                WHERE nickname = $NICKNAME AND ref_account_id = $REFACCOUNTID
            ";
            sqliteCommand.Parameters.AddWithValue("$NICKNAME", nickname);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refUserId.AccountId);

            SqliteDataReader dataReader = sqliteCommand.ExecuteReader();

            if (!dataReader.HasRows)
            {
                return null;
            }

            if (!dataReader.Read())
            {
                return null;
            }

            object[] rowColumns = new object[3];
            _ = dataReader.GetValues(rowColumns);

            Schema schema = new(rowColumns[0].ToString(), rowColumns[1].ToString(), rowColumns[2].ToString());
            return schema;
        }

        internal Schema? GetAccountEntry(UserId userId, UserId refUserId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                SELECT *
                FROM recipientaccounts
                WHERE account_id = $ACCOUNTID AND ref_account_id = $REFACCOUNTID
            ";
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", userId.AccountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refUserId.AccountId);

            SqliteDataReader dataReader = sqliteCommand.ExecuteReader();

            if (!dataReader.HasRows)
            {
                return null;
            }

            if (!dataReader.Read())
            {
                return null;
            }

            object[] rowColumns = new object[3];
            dataReader.GetValues(rowColumns);

            Schema schema = new(rowColumns[0].ToString(), rowColumns[1].ToString(), rowColumns[2].ToString());
            return schema;
        }

        internal void UpdateNickname(string nickname, UserId userId, UserId refUserId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"
                UPDATE recipientaccounts
                SET nickname = $NICKNAME
                WHERE account_id = $ACCOUNTID AND ref_account_id = $REFACCOUNTID
            ";
            sqliteCommand.Parameters.AddWithValue("$NICKNAME", nickname);
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", userId.AccountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refUserId.AccountId);

            sqliteCommand.ExecuteNonQuery();
        }
    }
}