using The_Project.Accounts;

namespace The_Project.Database
{
    internal interface IDatabaseAccount
    {
        public void CreateAccount(string Username, UserId UserId);
    }
}