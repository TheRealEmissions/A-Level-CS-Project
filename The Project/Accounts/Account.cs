﻿using Microsoft.Data.Sqlite;
using System;
using System.Net;
using The_Project.Database.Tables;
using The_Project.Exceptions;

#nullable enable

namespace The_Project.Accounts
{
    public class Account
    {
        public string Username { get; }
        public string AccountId { get; }
        public int MinPort { get; }
        public int MaxPort { get; }

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

            if (PasswordHash != ConfPasswordHash)
            {
                throw new PasswordHashMismatchException();
            }

            Database.UserAccount UserAccount = new(Connection, Tables);

            UserId UserId = new(Networking.Utils.GetIPAddress(), new Random().Next(10000, 30000), new Random().Next(60000, 99999), GenerateAccountId());
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
            UserId UserId = new(IPAddress.Parse("127.0.0.1"), MinPort, MaxPort, AccountId);
            return UserId;
        }
    }
}