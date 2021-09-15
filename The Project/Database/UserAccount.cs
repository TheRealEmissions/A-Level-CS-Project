using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Database.Interfaces;

namespace The_Project.Database
{
    class UserAccount : IDatabaseUserAccount
    {
        SqliteConnection Connection;

        public UserAccount(SqliteConnection Connection)
        {
            this.Connection = Connection;
        }

        public UserAccount()
        {
            Connection = new SQL().Connection;
        }

        public bool ComparePassword(string Password)
        {
            string DbPassword = GetPassword();
            return DbPassword == Password;
        }

        public void CreateEntry()
        {

        }

        public string GetPassword()
        {
            throw new NotImplementedException();
        }
    }
}
