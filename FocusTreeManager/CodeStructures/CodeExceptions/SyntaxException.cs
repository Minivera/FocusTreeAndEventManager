using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    class SyntaxException : Exception
    {
        private string tag;

        private int? line;

        private int? column;

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
                    message += " at character " + line;
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
