using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    public class PotentiallySafeException : Exception
    {
        public bool isSafe = false;
    }
}
