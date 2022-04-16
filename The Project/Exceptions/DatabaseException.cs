using System;


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
