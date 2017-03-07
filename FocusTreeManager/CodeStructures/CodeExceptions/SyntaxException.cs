using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    class SyntaxException : Exception
    {
        private readonly string tag;

        private readonly int? line;

        private readonly int? column;

        public override string Message
        {
            get
            {
                //TODO: Add language support.
                string message = "Syntax error near " + tag;
                if (line != null)
                {
                    message += " on line " + line;
                }
                if (column != null)
                {
                    message += " at character " + column;
                }
                return message;
            }
        }

        public SyntaxException(string tag, int? line = null, int? column = null)
        {
            this.tag = tag;
            this.line = line;
            this.column = column;
        }
    }
}
