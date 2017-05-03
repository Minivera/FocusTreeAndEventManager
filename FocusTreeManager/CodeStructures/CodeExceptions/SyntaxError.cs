using System;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    public class SyntaxError
    {
        public string Message { get; set; }

        public int? Line { get; set; }

        public int? Column { get; set; }

        public string Tag { get; set; }

        public string InnerMessage { get; set; }

        public SyntaxError(string message)
        {
            Message = message;
            Line = null;
            Column = null;
            Tag = "";
            InnerMessage = Message;
        }

        public SyntaxError(string tag, int? line = null, int? column = null, 
                           Exception InnerException = null)
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
        }
    }
}
