using Microsoft.Data.Sqlite;
using System;
using The_Project.Database.Tables;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Accounts
{
    public class Account
    {
        public string Username { get; set; }
        public string AccountId { get; private set; } = string.Empty;
        public int MinPort { get; private set; }
        public int MaxPort { get; private set; }

        private readonly Tables Tables;

        // login into account and retrieve details
        public Account(string Username, string PasswordHash, SqliteConnection Connection, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;

            Login(Username, PasswordHash, Connection);
        }

        // register new account
        public Account(string Username, string PasswordHash, string ConfPasswordHash, SqliteConnection Connection, Tables Tables)
        {
            this.Username = Username;
            this.Tables = Tables;

            Register(Username, PasswordHash, ConfPasswordHash, Connection);
        }

        private void Login(string Username, string PasswordHash, SqliteConnection Connection)
        {
            UserAccount UserAccountTable = (UserAccount)Tables.GetTable("UserAccount");
            Database.UserAccount UserAccount = new(Connection, Tables);

            UserAccount.Schema? Entry = UserAccountTable.GetAccountEntry(Username);
            if (!Entry.HasValue)
            {
                throw new AccountNotFoundException("UserAccount");
            }
            AccountId = Entry.Value.AccountId;
            MinPort = new Random().Next(20000, 20005);
            MaxPort = new Random().Next(20006, 20010);

            bool PasswordIsCorrect = UserAccount.ComparePassword(this, PasswordHash);
            if (!PasswordIsCorrect)
            {
                throw new WrongPasswordException();
            }
        }

        private void Register(string Username, string PasswordHash, string ConfPasswordHash, SqliteConnection Connection)
        {
            if (PasswordHash != ConfPasswordHash)
            {
                throw new PasswordHashMismatchException();
            }

            Database.UserAccount UserAccount = new(Connection, Tables);
            Random random = new();
            UserId UserId = new(Networking.Utils.GetLocalIpAddress(), random.Next(19000, 19500), random.Next(19900, 21000), GenerateAccountId());
            MinPort = UserId.MinPort;
            MaxPort = UserId.MaxPort;
            AccountId = UserId.AccountId;

            UserAccount.CreateEntry(Username, PasswordHash, UserId);
        }

        private static string GenerateAccountId()
        {
            Random Random = new();
            string AccountId = string.Empty;

            string[] Alphabet = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            for (int i = 0; i < 3; i++)
            {
                AccountId += Alphabet[Random.Next(1, 52) - 1];
            }
            return AccountId;
        }

        public UserId ToUserId()
        {
            UserId UserId = new(Networking.Utils.GetLocalIpAddress(), MinPort, MaxPort, AccountId);
            return UserId;
        }
    }
}