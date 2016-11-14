using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    struct ErrorStruct
    {
        public int ErrorNumber;
        public string ClassName;
        public string Line;
        public string Message;
        public string TimeStamp;
    }

    /// <summary>
    /// Special exception that builds itself as it is thrown. 
    /// Allows to build a custom stacktrace in the code analysis and
    /// parsing functions.
    /// </summary>
    class RecursiveCodeException : Exception
    {
        private List<ErrorStruct> Messages = new List<ErrorStruct>();

        public override string Message
        {
            get
            {
                string message = "";
                foreach (ErrorStruct item in Messages)
                {
                    message += item.TimeStamp + "# " + item.ErrorNumber
                        + " " + item.Message + " at " + item.ClassName +
                        " on line " + item.Line + "\n";
                }
                return message;
            }
        }

        public RecursiveCodeException AddToRecursiveChain(string message, string className, string Line)
        {
            ErrorStruct tempo = new ErrorStruct();
            tempo.ClassName = className;
            tempo.Message = message;
            tempo.Line = Line;
            tempo.ErrorNumber = Messages.Count + 1;
            tempo.TimeStamp = "[" + DateTime.Now.ToString("h:mm:ss tt") + "]";
            Messages.Add(tempo);
            return this;
        }

    }
}
