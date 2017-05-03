using System;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.CodeStructures.CodeExceptions
{
    internal struct ErrorStruct
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
    public class RecursiveCodeLog
    {
        private readonly List<ErrorStruct> Messages = new List<ErrorStruct>();

        public string Message
        {
            get
            {
                return Messages.Aggregate("", (current, item) => 
                    current + item.TimeStamp + "# " + 
                    item.ErrorNumber + " " + item.Message + " near " + 
                    item.ClassName + " on line " + item.Line + "\n");
            }
        }

        public RecursiveCodeLog AddToRecursiveChain(string message, string className, 
            string Line)
        {
            ErrorStruct tempo = new ErrorStruct
            {
                ClassName = className,
                Message = message,
                Line = Line,
                ErrorNumber = Messages.Count + 1,
                TimeStamp = "[" + DateTime.Now.ToString("h:mm:ss tt") + "]"
            };
            Messages.Add(tempo);
            return this;
        }

    }
}
