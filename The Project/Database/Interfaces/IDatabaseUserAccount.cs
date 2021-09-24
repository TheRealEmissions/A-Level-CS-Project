using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Database.Interfaces
{
    /**
     * REQUIRED:
     * check password (based on hashes)
     */
    internal interface IDatabaseUserAccount : IDatabaseAccount
    {
        public bool ComparePassword(string Password);
        public string GetPassword();
        public void SetPassword();
    }
}
