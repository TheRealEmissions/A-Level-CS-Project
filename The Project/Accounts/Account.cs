using Microsoft.Data.Sqlite;
using System;
using The_Project.Cryptography;
using The_Project.Database.Tables;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Accounts
{
    public sealed class Account
    {
        internal string Username { get; }
        internal string AccountId { get; private set; } = string.Empty;
        private int MinPort { get; set; }
        private int MaxPort { get; set; }

        internal PublicKey PublicKey { get; private set; }
        private PrivateKey PrivateKey { get; set; }

        private readonly Tables _tables;

        // login into account and retrieve details
        internal Account(string username, string passwordHash, SqliteConnection connection, Tables tables)
        {
            Username = username;
            _tables = tables;

            Login(username, passwordHash, connection);
        }

        // register new account
        internal Account(string username, string passwordHash, string confPasswordHash, SqliteConnection connection,
            Tables tables)
        {
            Username = username;
            _tables = tables;

            if (passwordHash != confPasswordHash)
            {
                throw new PasswordHashMismatchException();
            }

            Register(username, passwordHash, connection);
        }

        internal void SetPublicKey(PublicKey publicKey)
        {
            PublicKey = publicKey;
        }

        internal void SetPrivateKey(PrivateKey privateKey)
        {
            PrivateKey = privateKey;
        }

        private void Login(string username, string passwordHash, SqliteConnection connection)
        {
            UserAccount userAccountTable = (UserAccount) _tables.GetTable("UserAccount");
            Database.UserAccount userAccount = new(_tables);

            UserAccount.UserAccountSchema? accountEntry = userAccountTable.GetAccountEntry(username);
            if (!accountEntry.HasValue)
            {
                throw new AccountNotFoundException("UserAccount");
            }

            AccountId = accountEntry.Value.AccountId;
            MinPort = new Random().Next(20000, 20005);
            MaxPort = new Random().Next(20006, 20010);

            bool passwordIsCorrect = userAccount.ComparePassword(this, passwordHash);
            if (!passwordIsCorrect)
            {
                throw new WrongPasswordException(userAccount.GetPassword(this));
            }
        }

        private void Register(string username, string passwordHash, SqliteConnection connection)
        {
            Database.UserAccount userAccount = new(_tables);
            Random random = new();
            UserId userId = new(Networking.Utils.GetLocalIpAddress(), random.Next(19000, 19500),
                random.Next(19900, 21000), GenerateAccountId());
            MinPort = userId.MinPort;
            MaxPort = userId.MaxPort;
            AccountId = userId.AccountId;

            userAccount.CreateEntry(username, passwordHash, userId);
        }

        private static string GenerateAccountId()
        {
            Random random = new();
            string accountId = string.Empty;

            string[] alphabet =
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
            };

            for (int i = 0; i < 3; i++)
            {
                accountId += alphabet[random.Next(1, 52) - 1];
            }

            return accountId;
        }

        internal UserId ToUserId()
        {
            UserId userId = new(Networking.Utils.GetLocalIpAddress(), MinPort, MaxPort, AccountId);
            return userId;
        }
    }
}