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

        public void CreateAccount(string username, string accountId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount) _tables.GetTable("RecipientAccount");
            bool createdEntry = table.CreateAccountEntry(username, accountId, _userAccountInstance.AccountId);
            if (!createdEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public void CreateAccount(string accountId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount) _tables.GetTable("RecipientAccount");
            bool createdEntry = table.CreateAccountEntry(accountId, _userAccountInstance.AccountId);
            if (!createdEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public Tables.RecipientAccount.RecipientAccountSchema? GetAccount(string nicknameOrAccountId, bool usingNickname = false)
        {
            Tables.RecipientAccount recipientAccount = (Tables.RecipientAccount) _tables.GetTable("RecipientAccount");
            Tables.RecipientAccount.RecipientAccountSchema? accountEntry =
                recipientAccount.GetAccountEntry(nicknameOrAccountId, _userAccountInstance.ToUserId(), usingNickname);
            return accountEntry;
        }

        public bool HasAccount(string nicknameOrAccountId, bool usingNickname = false)
        {
            Tables.RecipientAccount.RecipientAccountSchema? account = GetAccount(nicknameOrAccountId, usingNickname);
            return account is not null;
        }

        public void UpdateNickname(string nickname, string accountId)
        {
            Tables.RecipientAccount table = (Tables.RecipientAccount) _tables.GetTable("RecipientAccount");
            table.UpdateNickname(nickname, accountId, _userAccountInstance.ToUserId());
        }
    }
}