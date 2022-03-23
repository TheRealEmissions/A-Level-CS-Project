using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using The_Project.Database.Tables.Interfaces;

namespace The_Project.Database.Tables
{
    public class Tables
    {
        private readonly List<KeyValuePair<string, ISQLTable>> AllTables = new();

        public Tables(SqliteConnection Connection)
        {
            AllTables.Add(KeyValuePair.Create<string, ISQLTable>("Messages", new Messages(Connection)));
            AllTables.Add(KeyValuePair.Create<string, ISQLTable>("RecipientAccount", new RecipientAccount(Connection)));
            AllTables.Add(KeyValuePair.Create<string, ISQLTable>("UserAccount", new UserAccount(Connection)));
        }

        public ISQLTable GetTable(string Key)
        {
            return AllTables.Find(x => x.Key == Key).Value;
        }

        public List<KeyValuePair<string, ISQLTable>> GetAllTables()
        {
            return AllTables;
        }
    }
}