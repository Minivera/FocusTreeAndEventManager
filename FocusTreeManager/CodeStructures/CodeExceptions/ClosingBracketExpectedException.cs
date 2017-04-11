using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class ClosingBracketExpectedException : Exception
    {
        public override string Message => "No associated closing bracket exists, " +
                                          "all opening bracket must be closed.";
    }
}
