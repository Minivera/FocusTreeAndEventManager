namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class MandatoryTagException: PotentiallySafeException
    {
        private readonly string Tag;

        public override string Message => "The expression "
            + Tag + " is mandatory, it must be defined for the script to be parsed.";

        public MandatoryTagException(string Tag)
        {
            this.Tag = Tag;
        }
    }
}
