using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class IncompleteExpressionException: Exception
    {
        public override string Message => "The expression is incomplete, " +
                                          "an operator and a value was expected.";
    }
}
