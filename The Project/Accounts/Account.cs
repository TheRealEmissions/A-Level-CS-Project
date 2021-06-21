using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Accounts
{
    class Account : AccountHandler
    {
        public Account(string Username, string Password) : base(Username, Password)
        {

        }

        public Account(string Username, string Password, string ConfPassword) : base(Username, Password, ConfPassword)
        {

        }
    }
}
