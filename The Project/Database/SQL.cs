using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using The_Project.Database.Tables.Interfaces;

#nullable enable

namespace The_Project.Database
{
    public class SQL
    {
        public SqliteConnection Connection { get; private set; }
        public Tables.Tables Tables { get; private set; }

        public SQL()
        {
            Connection = Start();
            Tables = new(Connection);
            CreateTables();
        }

        private static SqliteConnection Start()
        {
            SqliteConnection Connection = new(new SqliteConnectionStringBuilder() { DataSource = "database.db", ForeignKeys = true, Mode = SqliteOpenMode.ReadWriteCreate }.ToString());
            Connection.Open();
            return Connection;
        }

        private void CreateTables()
        {
            List<KeyValuePair<string, ISQLTable>> DbTables = Tables.GetAllTables();
            foreach (KeyValuePair<string, ISQLTable> Table in DbTables)
            {
                Table.Value.CreateTable();
            }
        }
    }
}