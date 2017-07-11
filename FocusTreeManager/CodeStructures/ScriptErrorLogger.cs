using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusTreeManager.CodeStructures.CodeExceptions;

namespace FocusTreeManager.CodeStructures
{
    public class ScriptErrorLogger
    {
        public List<SyntaxError> Errors { get; set; }

        public ScriptErrorLogger()
        {
            Errors = new List<SyntaxError>();
        }

        public bool hasErrors()
        {
            return Errors.Any(e => !e.isSafe);
        }

        public bool hasErrorsAndSafeErrors()
        {
            return Errors.Any();
        }

        public List<SyntaxError> getErrors()
        {
            List<SyntaxError> newlist = Errors.ToList();
            Errors.Clear();
            return newlist;
        }

        public string ErrorsToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (SyntaxError error in Errors)
            {
                builder.AppendLine(error.Message);
            }
            return builder.ToString();
        }
    }
}