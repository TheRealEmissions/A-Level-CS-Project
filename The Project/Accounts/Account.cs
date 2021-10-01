using Microsoft.Data.Sqlite;
using The_Project.Database.Tables;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Accounts
{
    public class Account
    {
        public string Username { get; }
        public string AccountId { get; }

        private readonly Tables Tables;

        // login into account and retrieve details
        public Account(string Username, string PasswordHash, SqliteConnection Connection, Tables Tables)
        {
            this.Tables = Tables;

            UserAccount UserAccountTable = (UserAccount)Tables.GetTable("UserAccount");
            Database.UserAccount UserAccount = new(Connection, Tables);

            UserAccount.Schema? Entry = UserAccountTable.GetAccountEntry(Username);
            if (!Entry.HasValue)
            {
                throw new AccountNotFoundException("UserAccount");
            }

            this.Username = Username;
            AccountId = Entry.Value.AccountId;

            bool PasswordIsCorrect = UserAccount.ComparePassword(this, PasswordHash);
            if (!PasswordIsCorrect)
            {
                throw new WrongPasswordException();
            }

        }

        // register new account
        public Account(string Username, string PasswordHash, string ConfPasswordHash, SqliteConnection Connection, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;
        }
    }
}