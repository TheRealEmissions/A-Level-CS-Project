using The_Project.Accounts;

namespace The_Project.Database.Interfaces
{
    /**
     * REQUIRED:
     * check password (based on hashes)
     */

    internal interface IDatabaseUserAccount
    {
        public bool ComparePassword(Account account, string passwordHash);

        public string GetPassword(Account account);

        public void SetPassword(Account account, string passwordHash);

        public void CreateEntry(string username, string passwordHash, UserId userId);
    }
}