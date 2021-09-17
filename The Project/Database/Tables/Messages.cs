using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public class Messages : MustConstructWith<SqliteConnection>, ISQLTable
    {
        protected SqliteConnection Connection;
        public Messages(SqliteConnection Connection) : base(Connection)
        {
            this.Connection = Connection;
            CreateTable();
        }

        public void CreateTable()
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                CREATE TABLE IF NOT EXISTS db.messages (
                    user_account_id TEXT NOT NULL
                )
            ";
        }
    }
}
