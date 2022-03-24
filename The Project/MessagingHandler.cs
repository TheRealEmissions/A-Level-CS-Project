using The_Project.Accounts;
using The_Project.Database;

#nullable enable

namespace The_Project
{
    public class MessagingHandler : Sql
    {
        public Account? UserAccount { get; set; }
        public Recipient? Recipient { get; set; }
    }
}