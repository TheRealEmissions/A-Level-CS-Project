using Microsoft.Data.Sqlite;
using System.Net;
using The_Project.Database.Tables.Interfaces;
using The_Project.Extensions;

namespace The_Project.Database.Tables
{
    public class RecipientAddresses : MustConstructWith<SqliteConnection>, ISQLTable
    {
        private readonly SqliteConnection Connection;

        public RecipientAddresses(SqliteConnection Connection) : base(Connection)
        {
            this.Connection = Connection;
        }

        public struct Schema
        {
            public IPAddress IPAddress { get; }
            public int MinPort { get; }
            public int MaxPort { get; }
            public string AccountId { get; }
            public string RefAccountId { get; }

            public Schema(int OctetOne, int OctetTwo, int OctetThree, int OctetFour, int MinPort, int MaxPort, string AccountId, string RefAccountId)
            {
                this.IPAddress = IPAddress.Parse($"{OctetOne}.{OctetTwo}.{OctetThree}.{OctetFour}");
                this.MinPort = MinPort;
                this.MaxPort = MaxPort;
                this.AccountId = AccountId;
                this.RefAccountId = RefAccountId;
            }
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
                    account_id TEXT NOT NULL,
                    ref_account_id TEXT NOT NULL UNIQUE,
                    FOREIGN KEY (ref_account_id)
                        REFERENCES recipientaccount (ref_account_id)
                            ON DELETE CASCADE
                            ON UPDATE CASCADE
                )
            ";
            Command.ExecuteNonQuery();
        }
    }
}