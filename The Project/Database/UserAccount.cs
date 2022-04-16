using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Interfaces;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Database
{
    internal sealed class UserAccount : IDatabaseUserAccount
    {
        private readonly Tables.Tables _tables;

        internal UserAccount(Tables.Tables tables)
        {
            _tables = tables;
        }

        public bool ComparePassword(Account account, string passwordHash)
        {
            return GetPassword(account) == passwordHash;
        }

        public void CreateEntry(string username, string passwordHash, UserId userId)
        {
            Tables.UserAccount userAccount = (Tables.UserAccount) _tables.GetTable("UserAccount");
            bool createdEntry = userAccount.CreateAccountEntry(username, passwordHash, userId.AccountId);
            if (!createdEntry)
            {
                throw new AccountCreationException("ACCOUNT NOT CREATED");
            }
        }

        public Tables.UserAccount.UserAccountSchema? GetAccount(string username)
        {
            Tables.UserAccount userAccount = (Tables.UserAccount) _tables.GetTable("UserAccount");
            Tables.UserAccount.UserAccountSchema? accountEntry = userAccount.GetAccountEntry(username);
            return accountEntry;
        }

        public string? GetPassword(Account account)
        {
            Tables.UserAccount.UserAccountSchema? entry = GetAccount(account.Username);
            return entry?.Password;
        }

        public void SetPassword(Account account, string passwordHash)
        {
            Tables.UserAccount userAccount = (Tables.UserAccount) _tables.GetTable("UserAccount");
            bool updated = userAccount.UpdatePasswordInEntry(account.AccountId, passwordHash);
            if (!updated)
            {
                throw new PasswordUpdateException("Password not updated in entry");
            }
        }

        public void AddMessage(Tables.Messages.MessageSchema messageMessageSchema)
        {
            Tables.Messages messages = (Tables.Messages) _tables.GetTable("Messages");
            bool updated = messages.CreateMessageEntry(messageMessageSchema.UserAccountId, messageMessageSchema.RefAccountId,
                messageMessageSchema.Timestamp, messageMessageSchema.Message, messageMessageSchema.Received);
            if (!updated)
            {
                throw new DatabaseException("Message not added to database!");
            }
        }
    }
}