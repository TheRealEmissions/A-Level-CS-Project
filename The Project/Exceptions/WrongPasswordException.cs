using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Exceptions
{
    [Serializable]
    class WrongPasswordException : Exception
    {
        public WrongPasswordException() : base("WRONG PASSWORD") { }
    }
}
