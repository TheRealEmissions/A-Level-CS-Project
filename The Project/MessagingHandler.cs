using The_Project.Accounts;
using The_Project.Database;

#nullable enable

namespace The_Project
{
    internal sealed class MessagingHandler : Sql
    {
        internal Account? UserAccount { get; set; }
        internal Recipient? Recipient { get; set; }
    }
}