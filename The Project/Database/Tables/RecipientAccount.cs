using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public class RecipientAccount : MustConstructWith<SqliteConnection>, ISqlTable
    {
        private readonly SqliteConnection _sqliteConnection;

        public RecipientAccount(SqliteConnection sqliteConnection) : base(sqliteConnection)
        {
            _sqliteConnection = sqliteConnection;
            //CreateTable();
        }

        public struct Schema
        {
            public string Nickname { get; }
            public string AccountId { get; }
            public string RefAccountId { get; }

            public Schema(string nickname, string accountId, string refAccountId)
            {
                this.Nickname = nickname;
                this.AccountId = accountId;
                this.RefAccountId = refAccountId;
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
            sqliteCommand.CommandText = sqliteCommand.CommandText.Replace("$database", _sqliteConnection.Database + ".recipientaccounts");
            //Command.Parameters.AddWithValue("$database", _sqliteConnection.Database + ".recipientaccounts");
            sqliteCommand.ExecuteNonQuery();
        }

        public bool CreateAccountEntry(string nickname, string accountId, string refAccountId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"INSERT INTO recipientaccounts (nickname, account_id, ref_account_id) VALUES ($NICKNAME, $ACCOUNTID, $REFACCOUNTID)";
            sqliteCommand.Parameters.AddWithValue("$NICKNAME", nickname);
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refAccountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }

        public bool CreateAccountEntry(string accountId, string refAccountId)
        {
            SqliteCommand sqliteCommand = _sqliteConnection.CreateCommand();
            sqliteCommand.CommandText = @"INSERT INTO recipientaccounts (account_id, ref_account_id) VALUES ($ACCOUNTID, $REFACCOUNTID)";
            sqliteCommand.Parameters.AddWithValue("$ACCOUNTID", accountId);
            sqliteCommand.Parameters.AddWithValue("$REFACCOUNTID", refAccountId);
            int rows = sqliteCommand.ExecuteNonQuery();
            return rows > 0;
        }

        public Schema? GetAccountEntry(string nickname, UserId refUserId)
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

            if (dataReader.Read())
            {
                object[] rowColumns = new object[3];
                dataReader.GetValues(rowColumns);

                Schema schema = new(nickname: rowColumns[0].ToString(), accountId: rowColumns[1].ToString(), refAccountId: rowColumns[2].ToString());
                return schema;
            }

            return null;
        }

        public Schema? GetAccountEntry(UserId userId, UserId refUserId)
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

            if (dataReader.Read())
            {
                object[] rowColumns = new object[3];
                dataReader.GetValues(rowColumns);

                Schema schema = new(nickname: rowColumns[0].ToString(), accountId: rowColumns[1].ToString(), refAccountId: rowColumns[2].ToString());
                return schema;
            }

            return null;
        }

        public void UpdateNickname(string nickname, UserId userId, UserId refUserId)
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