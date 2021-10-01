using Microsoft.Data.Sqlite;
using System;
using The_Project.Accounts;
using The_Project.Database.Interfaces;

#nullable enable

namespace The_Project.Database
{
    public class UserAccount : IDatabaseUserAccount
    {
        private readonly SqliteConnection Connection;
        private readonly Tables.Tables Tables;

        public UserAccount(SqliteConnection Connection, Tables.Tables Tables)
        {
            this.Connection = Connection;
            this.Tables = Tables;
        }

        public UserAccount(Tables.Tables Tables)
        {
            Connection = new SQL().Connection;
            this.Tables = Tables;
        }

        public bool ComparePassword(Account Account, string Password)
        {
            return GetPassword(Account) == Password;
        }

        public void CreateEntry(string Username, UserId UserId)
        {
        }

        public Tables.UserAccount.Schema? GetAccount(string Username)
        {
            Tables.UserAccount UserAccountDb = (Tables.UserAccount)Tables.GetTable("UserAccount");
            Tables.UserAccount.Schema? Entry = UserAccountDb.GetAccountEntry(Username);
            return Entry;
        }

        public string GetPassword(Account Account)
        {
            SqliteCommand Command = new();
            Command.CommandText = @"SELECT ";
        }

        public void SetPassword(Account Account)
        {
            throw new NotImplementedException();
        }
    }
}