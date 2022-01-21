using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Interfaces;
using The_Project.Exceptions;

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

        public bool ComparePassword(Account Account, string PasswordHash)
        {
            return GetPassword(Account) == PasswordHash;
        }

        public void CreateEntry(string Username, string PasswordHash, UserId UserId)
        {
            Tables.UserAccount UserAccountDb = (Tables.UserAccount)Tables.GetTable("UserAccount");
            bool CreatedEntry = UserAccountDb.CreateAccountEntry(Username, PasswordHash, UserId.AccountId);
            if (!CreatedEntry)
            {
                throw new AccountCreationException("ACCOUNT NOT CREATED");
            }
        }

        public Tables.UserAccount.Schema? GetAccount(string Username)
        {
            Tables.UserAccount UserAccountDb = (Tables.UserAccount)Tables.GetTable("UserAccount");
            Tables.UserAccount.Schema? Entry = UserAccountDb.GetAccountEntry(Username);
            return Entry;
        }

        public string GetPassword(Account Account)
        {
            Tables.UserAccount.Schema Entry = (Tables.UserAccount.Schema)GetAccount(Account.Username);
            return Entry.Password;
        }

        public void SetPassword(Account Account, string PasswordHash)
        {
            Tables.UserAccount Table = (Tables.UserAccount)Tables.GetTable("UserAccount");
            Table.UpdatePasswordInEntry(Account.AccountId, PasswordHash);
        }
    }
}