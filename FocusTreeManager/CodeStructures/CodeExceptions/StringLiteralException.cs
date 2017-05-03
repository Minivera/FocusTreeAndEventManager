using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal class StringLiteralException : Exception
    {
        public override string Message => "No closing quotation mark was found, " + 
                                          "all string literal must be closed.";
    }
}
