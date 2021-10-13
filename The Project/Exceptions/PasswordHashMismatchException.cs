using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Exceptions
{
    class PasswordHashMismatchException : Exception
    {
        public string Type { get; }

        public PasswordHashMismatchException() : base("ACCOUNT CREATION") { }

        public PasswordHashMismatchException(string Type) : this()
        {
            this.Type = Type;
        }
    }
}
