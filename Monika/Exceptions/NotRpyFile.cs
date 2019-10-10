using System;

namespace Monika.Exceptions
{
    [Serializable]
    class NotRpyFile : Exception
    {
        public NotRpyFile()
            : base("This file is not a Rpy file.")
        {}
    }
}