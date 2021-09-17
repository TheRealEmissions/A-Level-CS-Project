using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;

namespace The_Project.Database
{
    interface IDatabaseAccount
    {
        public void CreateEntry(string Username, UserId UserId);
    }
}
