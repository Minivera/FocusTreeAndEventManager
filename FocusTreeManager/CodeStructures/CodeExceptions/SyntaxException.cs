using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    class SyntaxException : Exception
    {
        private readonly string tag;

        private readonly int? line;

        private readonly int? column;

        private readonly string InnerMessage;

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
                if (!string.IsNullOrEmpty(InnerMessage))
                {
                    message += "\n" + InnerMessage;
                }
                return message;
            }
        }

        public SyntaxException(string tag, int? line = null, int? column = null
            , Exception InnerException = null)
        {
            this.tag = tag;
            this.line = line;
            this.column = column;
            InnerMessage = InnerException?.Message;
        }
    }
}
