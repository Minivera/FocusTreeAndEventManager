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
        private readonly Brush KeyWordColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#3f51b5");

        private readonly Brush AssignerColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#ffeb3b");

        private readonly Brush ConditionColor = (SolidColorBrush)
            new BrushConverter().ConvertFrom("#b71c1c");

        private static readonly Lazy<CodeEditorContent> lazy =
        new Lazy<CodeEditorContent>(() => new CodeEditorContent());

        public static CodeEditorContent Instance => lazy.Value;

        public List<string> KeyWords { get; }

        public List<string> Conditions { get; }

        public List<string> Assigners { get; }

        public Dictionary<string, string> Snippets { get; }

        public CodeEditorContent()
        {
            Assigners = new List<string>();
            Conditions = new List<string>();
            KeyWords = new List<string>();
            Snippets = new Dictionary<string, string>();
            LoadList(@"Common\ScripterSymbols\Assigners.xml", Assigners);
            LoadList(@"Common\ScripterSymbols\Conditions.xml", Conditions);
            LoadList(@"Common\ScripterSymbols\Keywords.xml", KeyWords);
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

        public bool IsKnownKeyword(string tag)
        {
            return KeyWords.Exists((s) => s.ToLower().Equals(tag, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsKnownAssigner(string tag)
        {
            return Assigners.Exists((s) => s.ToLower().Equals(tag, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsKnownCondition(string tag)
        {
            return Conditions.Exists((s) => s.ToLower().Equals(tag, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        public bool IsKnownSnippet(string tag)
        {
            return Snippets.Keys.ToList().Exists((s) => s.ToLower().Equals(tag, 
                StringComparison.InvariantCultureIgnoreCase));
        }

        public void Highlight(FormattedText text, int openingBracketPos, 
            int ClosingBracketPos, Brush DelimiterBrush)
        {
            Regex wordsRgx = new Regex(@"[a-zA-Z_\{\}][a-zA-Z0-9_\{\}]*");
            foreach (Match m in wordsRgx.Matches(text.Text))
            {
                if (IsKnownKeyword(m.Value))
                {
                    text.SetForegroundBrush(KeyWordColor, m.Index, m.Length);
                    text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                    text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
                }
                else if (IsKnownCondition(m.Value))
                {
                    text.SetForegroundBrush(ConditionColor, m.Index, m.Length);
                    text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                    text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
                }
                else if(IsKnownAssigner(m.Value))
                {
                    text.SetForegroundBrush(AssignerColor, m.Index, m.Length);
                    text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                    text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
                }
                else if (m.Value == "{" && m.Index == openingBracketPos)
                {
                    text.SetForegroundBrush(DelimiterBrush, m.Index, m.Length);
                    text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                    text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
                }
                else if (m.Value == "}" && m.Index == ClosingBracketPos)
                {
                    text.SetForegroundBrush(DelimiterBrush, m.Index, m.Length);
                    text.SetFontWeight(FontWeights.Normal, m.Index, m.Length);
                    text.SetFontStyle(FontStyles.Normal, m.Index, m.Length);
                }
            }
        }
    }
}
