﻿using The_Project.Accounts;

namespace The_Project.Database.Interfaces
{
    /**
     * REQUIRED:
     * check password (based on hashes)
     */

    internal interface IDatabaseUserAccount
    {
        public bool ComparePassword(Account Account, string PasswordHash);

        public string GetPassword(Account Account);

        public void SetPassword(Account Account, string PasswordHash);

        public void CreateEntry(string Username, string PasswordHash, UserId UserId);
    }
}