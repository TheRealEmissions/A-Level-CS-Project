using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;
using The_Project.Database;

#nullable enable
namespace The_Project
{
    public class MessagingHandler : SQL
    {
        public Account? UserAccount { get; set; } = null;
        public Recipient? Recipient { get; set; } = null;

        public MessagingHandler() : base()
        {

        }
    }
}
