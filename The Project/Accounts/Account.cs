using Microsoft.Data.Sqlite;
using The_Project.Database.Tables;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Accounts
{
    public class Account
    {
        public string Username { get; } = "";

        private readonly Tables Tables;

        // login into account and retrieve details
        public Account(string Username, string Password, SqliteConnection Connection, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;

            Database.UserAccount UserAccountDb = (Database.UserAccount)Tables.GetTable("UserAccount");
            object[]? AccountColumns = UserAccountDb.GetAccount(Username);
            if (AccountColumns is null)
            {
                throw new AccountNotFoundException("UserAccount");
            }
        }

        // register new account
        public Account(string Username, string Password, string ConfPassword, SqliteConnection Connection, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;
        }

        // retrieve information for recipient account
        public Account(string Username, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;
        }
    }
}