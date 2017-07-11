namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class ClosingBracketExpectedException : PotentiallySafeException
    {
        public override string Message => "No associated closing bracket exists, " +
                                          "all opening bracket must be closed.";
    }
}
