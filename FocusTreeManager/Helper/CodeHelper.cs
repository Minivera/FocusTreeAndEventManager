using System;
using System.Linq;

namespace FocusTreeManager.Helper
{
	public class CodeHelper
    {
		/// <summary>
		/// Returns the raw number of the current line count.
		/// </summary>
		public static int GetLineCount(String text)
        {
			int lcnt = 1;
			for (int i = 0; i < text.Length; i++)
            {
				if (text[i] == '\n')
                {
                    lcnt += 1;
                }
			}
			return lcnt;
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
                throw new ArgumentNullException("text");
            }
            if (lineIndex <= 0)
            {
                return 0;
            }
			int currentLineIndex = 0;
			for (int i = 0; i < text.Length - 1; i++)
            {
				if (text[i] == '\n')
                {
					currentLineIndex += 1;
                    if (currentLineIndex == lineIndex)
                    {
                        return Math.Min(i + 1, text.Length - 1);
                    }
				}
			}
			return Math.Max(text.Length - 1, 0);
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
                throw new ArgumentNullException("text");
            }
            if (lineIndex < 0)
            {
                return 0;
            }
			int currentLineIndex = 0;
			for (int i = 0; i < text.Length - 1; i++)
            {
				if (text[i] == '\n')
                {
                    if (currentLineIndex == lineIndex)
                    {
                        return i;
                    }
					currentLineIndex += 1;
				}
			}
			return Math.Max(text.Length - 1, 0);
		}

        public static int getLevelOfIndent(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
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
                //If a new opening bracket is found
                if (line.Contains("{"))
                {
                    //add one block opened
                    NumberOfBlocks++;
                }
                else if (line.Contains("}"))
                {
                    NumberOfBlocks--;
                }
                //If there was one closing bracket more than opening brackets
                if (line.Contains("}") && NumberOfBlocks <= 0)
                {
                    return ClosingPos + line.IndexOf("}") + 1;
                }
                else
                {
                    ClosingPos += line.Length + 1;
                }
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
                //If a new opening bracket is found
                if (line.Contains("}"))
                {
                    //add one block opened
                    NumberOfBlocks++;
                }
                else if (line.Contains("{"))
                {
                    NumberOfBlocks--;
                }
                //If there was one closing bracket more than opening brackets
                if (line.Contains("{") && NumberOfBlocks <= 0)
                {
                    return OpeningPos - (line.Reverse().ToList().IndexOf('{') + 1);
                }
                else
                {
                    OpeningPos -= (line.Length + 1);
                }
            }
            return OpeningPos;
        }
    }
}
