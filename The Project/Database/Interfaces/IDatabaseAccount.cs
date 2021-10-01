using The_Project.Accounts;

namespace The_Project.Database
{
    internal interface IDatabaseAccount
    {
        public void CreateEntry(string Username, UserId UserId);
    }
}