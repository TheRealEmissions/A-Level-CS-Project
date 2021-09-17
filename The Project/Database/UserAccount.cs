using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;
using The_Project.Database.Interfaces;

namespace The_Project.Database
{
    public class UserAccount : Tables.UserAccount, IDatabaseUserAccount
    {

        public UserAccount(SqliteConnection Connection) : base(Connection)
        {
        }

        public UserAccount() : base(new SQL().Connection)
        {
        }

        public bool ComparePassword(string Password)
        {
            string DbPassword = GetPassword();
            return DbPassword == Password;
        }

        public void CreateEntry(string Username, UserId UserId)
        {

        }

        public string GetPassword()
        {
            throw new NotImplementedException();
        }

        public void SetPassword()
        {
            throw new NotImplementedException();
        }
    }
}
