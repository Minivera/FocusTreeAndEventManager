using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    public class SyntaxError
    {
        public string Message { get; }

        public int? Line { get; private set; }

        public int? Column { get; private set; }

        public string Tag { get; private set; }

        public string InnerMessage { get; private set; }

        public bool isSafe { get; private set; }

        public SyntaxError(string message)
        {
            Message = message;
            Line = null;
            Column = null;
            Tag = "";
            InnerMessage = Message;
        }

        public SyntaxError(string tag, int? line = null, int? column = null, 
                           PotentiallySafeException InnerException = null)
        {
            Message = "Syntax error near " + tag;
            InnerMessage = Message;
            if (line != null)
            {
                Message += " on line " + line;
            }
            if (column != null)
            {
                Message += " at character " + column;
            }
            if (InnerException != null)
            {
                Message += "\n" + InnerException.Message;
                InnerMessage = InnerException.Message;
            }
            Line = line;
            Column = column;
            Tag = tag;
            isSafe = InnerException?.isSafe ?? false;
        }
    }
}
