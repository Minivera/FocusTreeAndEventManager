using System;
using System.Linq;

namespace FocusTreeManager.Helper
{
	public class CodeHelper
    {
		/// <summary>
		/// Returns the raw number of the current line count.
		/// </summary>
		public static int GetLineCount(string text)
		{
		    return text.Count(t => t == '\n');
		}

		/// <summary>
		/// Returns the index of the first character of the
		/// specified line. If the index is greater than the current
		/// line count, the method returns the index of the last
		/// character. The line index is zero-based.
		/// </summary>
		public static int GetFirstCharIndexFromLineIndex(string text, int lineIndex)
        {
			if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (lineIndex <= 0)
            {
                return 0;
            }
			int currentLine = 0;
            int charindex = 0;
            foreach (string line in text.Split('\n'))
            {
                if (currentLine == lineIndex)
                {
                    while (char.IsWhiteSpace(text[charindex]))
                    {
                        charindex++;
                    }
                    return charindex;
                }
                currentLine++;
                charindex += line.Length + 1;
            }
            return 0;
        }

		/// <summary>
		/// Returns the index of the last character of the
		/// specified line. If the index is greater than the current
		/// line count, the method returns the index of the last
		/// character. The line-index is zero-based.
		/// </summary>
		public static int GetLastCharIndexFromLineIndex(string text, int lineIndex)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (lineIndex < 0)
            {
                return 0;
            }
            int currentLine = 0;
            int charindex = 0;
            foreach (string line in text.Split('\n'))
            {
                if (currentLine == lineIndex)
                {
                    return charindex + line.Length - 1;
                }
                charindex += line.Length + 1;
                currentLine++;
            }
            return text.Length;
        }

        public static int getLevelOfIndent(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            //Indent equals the number of opening brackets minus the number of closing brackets
            int IndentLevel = text.Count(f => f == '{') - text.Count(f => f == '}');
            return IndentLevel >= 0 ? IndentLevel : 0;
        }

        public static int getAssociatedClosingBracket(string text, int openingBracketPos)
        {
            int ClosingPos = openingBracketPos + text.Substring(openingBracketPos).Split('\n').First().Length;
            int NumberOfBlocks = 1;
            //For each line since the opening bracket
            foreach (string line in text.Substring(openingBracketPos).Split('\n').Skip(1))
            {
                //Count the number of closing bracket minus the number of opening bracket.
                NumberOfBlocks += line.Count(f => f == '{') - line.Count(f => f == '}');
                //If there was one closing bracket more than opening brackets
                if (line.Contains("}") && NumberOfBlocks <= 0)
                {
                    return ClosingPos + line.IndexOf("}", StringComparison.Ordinal) + 1;
                }
                ClosingPos += line.Length + 1;
            }
            return ClosingPos;
        }

        public static int getAssociatedOpeningBracket(string text, int closingBracketPos)
        {
            int OpeningPos = closingBracketPos;
            int NumberOfBlocks = 1;
            //Reverse for each from the last closing bracket to beginning
            foreach (string line in text.Substring(0, closingBracketPos).Split('\n').Reverse())
            {
                //Count the number of closing bracket minus the number of opening bracket.
                NumberOfBlocks += line.Count(f => f == '}') - line.Count(f => f == '{');
                //If there was one closing bracket more than opening brackets
                if (line.Contains("{") && NumberOfBlocks <= 0)
                {
                    return OpeningPos - (line.Reverse().ToList().IndexOf('{') + 1);
                }
                OpeningPos -= line.Length + 1;
            }
            return OpeningPos;
        }

        public static int getStartCharOfPos(string text, int Line, int Column)
        {
            int LocalLine = 0;
            int Index = 0;
            foreach (string line in text.Split('\n'))
            {
                LocalLine++;
                if (LocalLine == Line)
                {
                    int localColumn = 0;
                    foreach (char c in line)
                    {
                        localColumn++;
                        Index++;
                        if (localColumn == Column)
                        {
                            return Index - 1;
                        }
                    }
                    break;
                }
                Index += line.Length + 1;
            }
            return -1;
        }
    }
}
