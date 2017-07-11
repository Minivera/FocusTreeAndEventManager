namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class OpeningBracketExpectedException: PotentiallySafeException
    {
        private readonly string ReceivedText;

        public override string Message => "An opening bracket was expected, "
            + ReceivedText + " was given.";

        public OpeningBracketExpectedException(string ReceivedText)
        {
            this.ReceivedText = ReceivedText;
        }
    }
}
