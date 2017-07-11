namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class UnparsableTagException: PotentiallySafeException
    {
        private readonly string Tag;

        public override string Message => "The expression "
            + Tag + " could not be parsed. Make sure it exists and is set correctly.";

        public UnparsableTagException(string Tag)
        {
            this.Tag = Tag;
        }
    }
}
