using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Database
{
    public class RecipientAccount : IDatabaseAccount
    {
        private readonly SqliteConnection Connection;

        public RecipientAccount(SqliteConnection Connection)
        {
            this.Connection = Connection;
        }

        public RecipientAccount()
        {
            Connection = new SQL().Connection;
        }

        public void CreateEntry(string Username, string Password)
        {

        }
    }
}
