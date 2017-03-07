using System.Windows;
using System.Windows.Media;

namespace FocusTreeManager.CodeStructures.CodeEditor
{
    public class InnerTextBlock
    {
        public string RawText { get; set; }

        public FormattedText FormattedText { get; set; }

        public FormattedText LineNumbers { get; set; }

        public int CharStartIndex { get; }

        public int CharEndIndex { get; }

        public int LineStartIndex { get; }

        public int LineEndIndex { get; }

        public Point Position => new Point(0, LineStartIndex * lineHeight);
        public bool IsLast { get; set; }

        public int Code { get; set; }

        private readonly double lineHeight;

        public InnerTextBlock(int charStart, int charEnd, int lineStart, 
            int lineEnd, double lineHeight)
        {
            CharStartIndex = charStart;
            CharEndIndex = charEnd;
            LineStartIndex = lineStart;
            LineEndIndex = lineEnd;
            this.lineHeight = lineHeight;
            IsLast = false;

        }

        public string GetSubString(string text)
        {
            return text.Substring(CharStartIndex, CharEndIndex - CharStartIndex + 1);
        }

        public override string ToString()
        {
            return $"L:{LineStartIndex}/{LineEndIndex} " +
                   $"C:{CharStartIndex}/{CharEndIndex} {FormattedText.Text}";
        }
    }
}
