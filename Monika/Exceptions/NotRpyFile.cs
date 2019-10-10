using System;

namespace Monika.Exceptions
{
    [Serializable]
    public class NotRpyFile : Exception
    {
        public NotRpyFile()
            : base("This file is not a Rpy file.")
        {}
    }
}