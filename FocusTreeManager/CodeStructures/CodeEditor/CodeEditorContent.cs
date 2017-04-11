using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace FocusTreeManager.CodeStructures.CodeEditor
{
    public sealed class CodeEditorContent
    {
        private readonly Brush StringColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#a5c25c");

        private readonly Brush BlockColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#6897bb");

        private readonly Brush AssignerColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#ffc66a");

        private readonly Brush NumberColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#cc7832");

        private readonly Brush BoolsColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#a3510d");

        private readonly Brush ErrorColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#8b0807");

        private readonly Brush CommentsColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#d2f776");

        private const string REGEX_STRINGS = "\"[A-Za-z\\s_-]*\"";

        private const string REGEX_BLOCS = @"([A-Za-z\t\r _-]*)[=|<|>]\s*\{";

        private const string REGEX_ASSIGN = @"([A-Za-z\t\r _-]*)[=|<|>][A-Za-z\t\r _-]*";

        private const string REGEX_NUMBERS = @"\d+(\.\d*[1-9])?";

        private const string REGEX_BOOLS = @"\b(yes|no|YES|NO|AND|and|OR|or|NOT|not)\b";

        private const string REGEX_COMMENTS = @"#.*\n";

        private static readonly Lazy<CodeEditorContent> lazy =
        new Lazy<CodeEditorContent>(() => new CodeEditorContent());

        public static CodeEditorContent Instance => lazy.Value;

        public Dictionary<string, string> Snippets { get; }

        public CodeEditorContent()
        {
            Snippets = new Dictionary<string, string>();
        }

        private static void LoadList(string filename, ICollection<string> listToModify)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            //Load all the children of the root node
            if (doc.DocumentElement != null)
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    listToModify.Add(node.InnerText);
                }
            }
        }

        public bool IsKnownSnippet(string tag)
        {
            return Snippets.Keys.ToList().Exists((s) => s.ToLower().Equals(tag, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        public void Highlight(FormattedText text, int openingBracketPos, 
            int ClosingBracketPos, Brush DelimiterBrush)
        {
            //The order is important so we overwrite when the color is to be overwritten 
            // (Numbers in string for example)
            Regex wordsRgx = new Regex(REGEX_ASSIGN);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                //This regex uses a matching group.
                text.SetForegroundBrush(AssignerColor, m.Groups[1].Index, m.Groups[1].Length);
                text.SetFontWeight(FontWeights.Normal, m.Groups[1].Index, m.Groups[1].Length);
                text.SetFontStyle(FontStyles.Normal, m.Groups[1].Index, m.Groups[1].Length);
            }
            wordsRgx = new Regex(REGEX_BLOCS);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                //This regex uses a matching group.
                text.SetForegroundBrush(BlockColor, m.Groups[1].Index, m.Groups[1].Length);
                text.SetFontWeight(FontWeights.Normal, m.Groups[1].Index, m.Groups[1].Length);
                text.SetFontStyle(FontStyles.Normal, m.Groups[1].Index, m.Groups[1].Length);
            }
            wordsRgx = new Regex(REGEX_BOOLS);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                text.SetForegroundBrush(BoolsColor, m.Index, m.Length);
                text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
            }
            wordsRgx = new Regex(REGEX_NUMBERS);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                text.SetForegroundBrush(NumberColor, m.Index, m.Length);
                text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
            }
            wordsRgx = new Regex(REGEX_STRINGS);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                text.SetForegroundBrush(StringColor, m.Index, m.Length);
                text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
            }
            wordsRgx = new Regex(REGEX_COMMENTS);
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                text.SetForegroundBrush(CommentsColor, m.Index, m.Length);
                text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
            }
            if (openingBracketPos > -1)
            {
                text.SetForegroundBrush(DelimiterBrush, openingBracketPos, 1);
                text.SetFontWeight(FontWeights.Normal, openingBracketPos, 1);
                text.SetFontStyle(FontStyles.Normal, openingBracketPos, 1);
            }
            if (ClosingBracketPos > -1)
            {
                text.SetForegroundBrush(DelimiterBrush, ClosingBracketPos, 1);
                text.SetFontWeight(FontWeights.Normal, ClosingBracketPos, 1);
                text.SetFontStyle(FontStyles.Normal, ClosingBracketPos, 1);
            }
        }
    }
}
