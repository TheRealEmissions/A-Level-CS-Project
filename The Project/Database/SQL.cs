using Microsoft.Data.Sqlite;

#nullable enable

namespace The_Project.Database
{
    public class SQL
    {
        public SqliteConnection Connection { get; set; }

        public SQL()
        {
            Connection = Start();
        }

        private static SqliteConnection Start()
        {
            SqliteConnection Connection = new("Data Source=account.db;Mode=ReadWriteCreate");
            Connection.Open();
            return Connection;
        }

    }
}