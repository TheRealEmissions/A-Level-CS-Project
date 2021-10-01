using The_Project.Accounts;

namespace The_Project.Database.Interfaces
{
    /**
     * REQUIRED:
     * check password (based on hashes)
     */

    internal interface IDatabaseUserAccount : IDatabaseAccount
    {
        public bool ComparePassword(string Password);

        public string GetPassword(Account Account);

        public void SetPassword(Account Account);
    }
}