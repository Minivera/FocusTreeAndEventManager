namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class OperatorExpectedException: PotentiallySafeException
    {
        private readonly string ReceivedText;

        public override string Message => "An operator (=, < or >) was expected, "
            + ReceivedText + " was given. The system will try loading with = as the operator.";

        public OperatorExpectedException(string ReceivedText)
        {
            this.ReceivedText = ReceivedText;
            this.isSafe = true;
        }
    }
}
