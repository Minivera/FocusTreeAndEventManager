namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class IncompleteExpressionException: PotentiallySafeException
    {
        public override string Message => "The expression is incomplete, " +
                                          "an operator and a value was expected." +
                                          " This code block will be ignored.";

        public IncompleteExpressionException()
        {
            isSafe = true;
        }
    }
}
