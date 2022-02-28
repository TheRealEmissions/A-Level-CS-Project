using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Exceptions;

namespace The_Project.Database
{
    public class RecipientAccount : IDatabaseAccount
    {
        private readonly SqliteConnection Connection;
        private readonly Account UserAccountInstance;
        private readonly Tables.Tables Tables;

        public RecipientAccount(SqliteConnection Connection, Account Account, Tables.Tables Tables)
        {
            this.Connection = Connection;
            this.UserAccountInstance = Account;
            this.Tables = Tables;
        }

        public void CreateAccount(string Nickname, UserId UserId)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            bool CreatedEntry = Table.CreateAccountEntry(Nickname, UserId.AccountId, UserAccountInstance.AccountId);
            if (!CreatedEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public void CreateAccount(UserId UserId)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            bool CreatedEntry = Table.CreateAccountEntry(UserId.AccountId, UserAccountInstance.AccountId);
            if (!CreatedEntry)
            {
                throw new AccountCreationException("RECIPIENT ACCOUNT NOT CREATED");
            }
        }

        public Tables.RecipientAccount.Schema? GetAccount(string Nickname)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            Tables.RecipientAccount.Schema? Entry = Table.GetAccountEntry(Nickname, UserAccountInstance.ToUserId());
            return Entry;
        }

        public Tables.RecipientAccount.Schema? GetAccount(UserId UserId)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            Tables.RecipientAccount.Schema? Entry = Table.GetAccountEntry(UserId, UserAccountInstance.ToUserId());
            return Entry;
        }

        public void UpdateNickname(string Nickname, UserId UserId)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            Table.UpdateNickname(Nickname, UserId, UserAccountInstance.ToUserId());
        }
    }
}