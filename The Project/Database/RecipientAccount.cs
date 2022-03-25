using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Interfaces;
using The_Project.Exceptions;

namespace The_Project.Database
{
    public class RecipientAccount : IDatabaseAccount
    {
        private readonly SqliteConnection _sqliteConnection;
        private readonly Account _userAccountInstance;
        private readonly Tables.Tables _tables;

        public RecipientAccount(SqliteConnection sqliteConnection, Account account, Tables.Tables tables)
        {
            _sqliteConnection = sqliteConnection;
            _userAccountInstance = account;
            _tables = tables;
        }

        public void CreateAccount(string username, UserId userId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount)_tables.GetTable("RecipientAccount");
            bool createdEntry = table.CreateAccountEntry(username, userId.AccountId, _userAccountInstance.AccountId);
            if (!createdEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public void CreateAccount(UserId userId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount)_tables.GetTable("RecipientAccount");
            bool createdEntry = table.CreateAccountEntry(userId.AccountId, _userAccountInstance.AccountId);
            if (!createdEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public Tables.RecipientAccount.Schema? GetAccount(string nickname)
        {
            Tables.RecipientAccount recipientAccount = (Tables.RecipientAccount)_tables.GetTable("RecipientAccount");
            Tables.RecipientAccount.Schema? accountEntry = recipientAccount.GetAccountEntry(nickname, _userAccountInstance.ToUserId());
            return accountEntry;
        }

        public Tables.RecipientAccount.Schema? GetAccount(UserId userId)
        {
            Tables.RecipientAccount recipientAccount = (Tables.RecipientAccount)_tables.GetTable("RecipientAccount");
            Tables.RecipientAccount.Schema? accountEntry = recipientAccount.GetAccountEntry(userId, _userAccountInstance.ToUserId());
            return accountEntry;
        }

        public void UpdateNickname(string nickname, UserId userId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount)_tables.GetTable("RecipientAccount");
            table.UpdateNickname(nickname, userId, _userAccountInstance.ToUserId());
        }
    }
}