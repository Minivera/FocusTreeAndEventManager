namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class StringLiteralException : PotentiallySafeException
    {
        public override string Message => "No closing quotation mark was found, " + 
                                          "all string literal must be closed.";
    }
}
