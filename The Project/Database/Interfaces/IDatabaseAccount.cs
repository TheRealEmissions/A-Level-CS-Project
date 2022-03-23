using The_Project.Accounts;

namespace The_Project.Database.Interfaces
{
    internal interface IDatabaseAccount
    {
        public void CreateAccount(string username, UserId userId);
    }
}