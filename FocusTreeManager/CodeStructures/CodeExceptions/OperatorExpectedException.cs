using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class OperatorExpectedException: Exception
    {
        private readonly string ReceivedText;

        public override string Message => "An operator (=, < or >) was expected, "
            + ReceivedText + " was given.";

        public OperatorExpectedException(string ReceivedText)
        {
            this.ReceivedText = ReceivedText;
        }
    }
}
