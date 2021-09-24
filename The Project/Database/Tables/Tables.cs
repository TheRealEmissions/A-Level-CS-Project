using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Database.Tables.Interfaces;

namespace The_Project.Database.Tables
{
    public class Tables
    {
        private readonly List<ISQLTable> AllTables = new();

        public Tables(SqliteConnection Connection)
        {
            AllTables.Add(new Messages(Connection));
            AllTables.Add(new RecipientAccount(Connection));
            AllTables.Add(new RecipientAddresses(Connection));
            AllTables.Add(new UserAccount(Connection));
        }

        public ISQLTable[] GetAndCreateAllTables()
        {
            return AllTables.ToArray();
        }
    }
}
