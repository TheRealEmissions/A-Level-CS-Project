using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;

namespace The_Project.Database
{
    public class RecipientAccount : Tables.RecipientAccount, IDatabaseAccount
    {
        private readonly SqliteConnection Connection;

        public RecipientAccount(SqliteConnection Connection) : base(Connection)
        {
            this.Connection = Connection;
        }

        public RecipientAccount() : base(new SQL().Connection)
        {
            Connection = new SQL().Connection;
        }

        public void CreateEntry(string Username, UserId UserId)
        {

        }
    }
}
