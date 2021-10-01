using Microsoft.Data.Sqlite;
using The_Project.Accounts;

namespace The_Project.Database
{
    public class RecipientAccount : IDatabaseAccount
    {
        private readonly SqliteConnection Connection;
        private readonly Account UserAccountInstance;
        private readonly Tables.Tables Tables;

        public RecipientAccount(SqliteConnection Connection, Account Account, Tables.Tables Tables)
        {
            this.Connection = Connection;
            this.UserAccountInstance = Account;
            this.Tables = Tables;
        }

        public RecipientAccount(Tables.Tables Tables)
        {
            Connection = new SQL().Connection;
            this.Tables = Tables;
        }

        public void CreateEntry(string Username, UserId UserId)
        {
            Tables.RecipientAccount Table = (Tables.RecipientAccount)Tables.GetTable("RecipientAccount");
            Table.CreateAccountEntry(Username, UserId.AccountId, UserAccountInstance.AccountId);
        }
    }
}