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
    public class RecipientAddresses : MustConstructWith<SqliteConnection>, ISQLTable
    {
        SqliteConnection Connection;
        public RecipientAddresses(SqliteConnection Connection) : base(Connection)
        {
            this.Connection = Connection;
        }

        public void CreateTable()
        {
            SqliteCommand Command = Connection.CreateCommand();
            Command.CommandText = @"
                CREATE TABLE IF NOT EXISTS db.recipientaddress (
                    octet_one INT NOT NULL,
                    octet_two INT NOT NULL,
                    octet_three INT NOT NULL,
                    octet_four INT NOT NULL,
                    min_port INT NOT NULL,
                    max_port INT NOT NULL,
                    account_id INT NOT NULL,
                    ref_account_id INT NOT NULL UNIQUE,
                    FOREIGN KEY (ref_account_id)
                        REFERENCES recipientaccount (ref_account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE
                )
            ";
            Command.ExecuteReader();
        }
    }
}
