using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using The_Project.Database.Tables.Interfaces;

#nullable enable

namespace The_Project.Database
{
    public class Sql
    {
        public SqliteConnection Connection { get; }
        public Tables.Tables Tables { get; }

        public Sql()
        {
            Connection = Start();
            Tables = new Tables.Tables(Connection);
            CreateTables();
        }

        private static SqliteConnection Start()
        {
            SqliteConnection sqliteConnection = new(new SqliteConnectionStringBuilder { DataSource = "database.db", ForeignKeys = true, Mode = SqliteOpenMode.ReadWriteCreate }.ToString());
            sqliteConnection.Open();
            return sqliteConnection;
        }

        private void CreateTables()
        {
            List<KeyValuePair<string, ISqlTable>> allTables = Tables.GetAllTables();
            foreach (KeyValuePair<string, ISqlTable> keyValuePair in allTables)
            {
                keyValuePair.Value.CreateTable();
            }
        }
    }
}