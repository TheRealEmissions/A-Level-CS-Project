using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Exceptions
{
    [Serializable]
    internal class ConnectionDeclinedException : Exception
    {
        public ConnectionDeclinedException(string error) : base(error)
        {

        }
    }
}
