using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class IncompleteExpressionException: Exception
    {
        public override string Message => "The exception is incomplete, " +
                                          "an operator and a value was expected.";
    }
}
