using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Exceptions
{
    public class AccountCreationException : Exception
    {
       public string Type { get; }

        public AccountCreationException() : base() { }

        public AccountCreationException(string Type) : base(Type)
        {
            this.Type = Type;
        }
        
    }
}
