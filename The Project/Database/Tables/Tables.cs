using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using The_Project.Database.Tables.Interfaces;

namespace The_Project.Database.Tables
{
    public class Tables
    {
        private readonly List<KeyValuePair<string, ISqlTable>> _allTables = new();

        public Tables(SqliteConnection connection)
        {
            _allTables.Add(KeyValuePair.Create<string, ISqlTable>("Messages", new Messages(connection)));
            _allTables.Add(KeyValuePair.Create<string, ISqlTable>("RecipientAccount", new RecipientAccount(connection)));
            _allTables.Add(KeyValuePair.Create<string, ISqlTable>("UserAccount", new UserAccount(connection)));
        }

        public ISqlTable GetTable(string key)
        {
            return _allTables.Find(x => x.Key == key).Value;
        }

        public List<KeyValuePair<string, ISqlTable>> GetAllTables()
        {
            return _allTables;
        }
    }
}