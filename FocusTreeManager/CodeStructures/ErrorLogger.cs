using FocusTreeManager.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Text;

namespace FocusTreeManager.CodeStructures
{
    public sealed class ErrorLogger
    {
        private static readonly Lazy<ErrorLogger> lazy =
        new Lazy<ErrorLogger>(() => new ErrorLogger());

        public static ErrorLogger Instance => lazy.Value;

        private readonly StringBuilder Error = new StringBuilder();

        public int NumberOfErrors { get; private set; }

        public string ErrorMessages => Error?.ToString() ?? "";

        private ErrorLogger()
        {
            NumberOfErrors = 0;
        }

        public void ClearLogging()
        {
            NumberOfErrors = 0;
            Error.Clear();
        }

        public void AddLogLine(string line)
        {
            Error.AppendLine(line);
            NumberOfErrors++;
            Messenger.Default.Send(new NotificationMessage(this, 
                (new ViewModelLocator()).ErrorDawg, "ErrorAdded"));
        }

        public bool hasErrors()
        {
            return Error.Length > 0;
        }
    }
}
