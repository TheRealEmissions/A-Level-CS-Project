using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class DatabaseException : Exception
    {
        public DatabaseException(string error) : base(error)
        {

        }

        public DatabaseException()
        {

        }
    }
}
