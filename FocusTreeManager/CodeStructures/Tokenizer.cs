using FocusTreeManager.CodeStructures.CodeExceptions;
using System.Collections.Generic;
using System.Linq;

namespace FocusTreeManager.CodeStructures
{
    public class Token
    {
        public string text { get; set; }

        public int line { get; set; }

        public int column { get; set; }
    }

    /// <summary>
    /// Class that store all tokens as a group of something =|<|> something else
    /// </summary>
    public class SyntaxGroup
    {
        public Token Component { get; set; }

        public Token Operator { get; set; }

        public object Operand { get; set; }
    }

    static class Tokenizer
    {
        static private readonly char[] delimiters_array = { '=', '<', '>', '{', '}' };

        static private char comment_char = '#';

        private static bool ContainsDelimiters(List<Token> tokens)
        {
            using (List<Token>.Enumerator enumerator = tokens.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.text.IndexOfAny(delimiters_array) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static int getIndexOfClosingBracket(List<Token> tokens)
        {
            int num = 1;
            int num2 = 0;
            while ((num > 0) && (num2 < tokens.Count))
            {
                if (tokens[num2].text == "{")
                {
                    num++;
                }
                else if (tokens[num2].text == "}")
                {
                    num--;
                }
                num2++;
            }
            if (num2 < 1)
            {
                return -1;
            }
            return num2;
        }

        public static object GroupTokensByBlocks(List<Token> tokens)
        {
            List<SyntaxGroup> list = new List<SyntaxGroup>();
            if (ContainsDelimiters(tokens))
            {
                while (tokens.Any<Token>())
                {
                    SyntaxGroup item = new SyntaxGroup
                    {
                        Component = tokens.First<Token>()
                    };
                    tokens.Remove(tokens.First<Token>());
                    if (((tokens.First<Token>().text != "=") && 
                        (tokens.First<Token>().text != "<")) && 
                        (tokens.First<Token>().text != ">"))
                    {
                        throw new SyntaxException(item.Component.text, 
                            new int?(tokens.First<Token>().line), new int?(tokens.First<Token>().column));
                    }
                    item.Operator = tokens.First<Token>();
                    tokens.RemoveAt(0);
                    if (tokens.First<Token>().text == "{")
                    {
                        tokens.RemoveAt(0);
                        int count = getIndexOfClosingBracket(tokens) - 1;
                        item.Operand = GroupTokensByBlocks(tokens.GetRange(0, count));
                        tokens.RemoveRange(0, count + 1);
                    }
                    else
                    {
                        if (tokens.First<Token>().text == "}")
                        {
                            throw new SyntaxException(item.Component.text, 
                                new int?(tokens.First<Token>().line), 
                                new int?(tokens.First<Token>().column));
                        }
                        item.Operand = tokens.First<Token>();
                        tokens.RemoveAt(0);
                    }
                    list.Add(item);
                }
                return list;
            }
            return tokens;
        }

        private static List<Token> SubTokenize(string text, int line, int column)
        {
            List<Token> list = new List<Token>();
            if (text.IndexOfAny(delimiters_array) != -1)
            {
                int num = column;
                string str = text.Substring(0, text.IndexOfAny(delimiters_array));
                list.AddRange(SubTokenize(str, line, num));
                text = text.Substring(text.IndexOfAny(delimiters_array));
                num += str.Length;
                Token item = new Token
                {
                    text = text.Substring(0, 1).Trim(),
                    line = line,
                    column = num
                };
                list.Add(item);
                text = text.Substring(1);
                num++;
                list.AddRange(SubTokenize(text, line, num));
                return list;
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                Token token2 = new Token
                {
                    text = text.Trim(),
                    line = line,
                    column = column
                };
                list.Add(token2);
            }
            return list;
        }

        public static List<Token> Tokenize(string text)
        {
            List<Token> list = new List<Token>();
            int line = 1;
            char[] separator = new char[] { '\n' };
            foreach (string str in text.Split(separator))
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    int column = 1;
                    bool flag = false;
                    string str2 = "";
                    bool flag2 = false;
                    char[] chArray2 = new char[] { ' ', '\t' };
                    foreach (string str3 in str.Split(chArray2))
                    {
                        string word = str3.Substring(0, str3.IndexOf(comment_char));
                        if (word.Contains<char>(comment_char))
                        {
                            flag2 = true;
                        }
                        if (word.Contains<char>('"') && !word.Contains<char>('\\'))
                        {
                            if (flag || (word.Count<char>(f => (f == '"')) > 1))
                            {
                                flag = false;
                                str2 = str2 + word.TrimEnd(new char[0]);
                                Token item = new Token
                                {
                                    text = str2,
                                    line = line,
                                    column = column
                                };
                                list.Add(item);
                                str2 = "";
                                continue;
                            }
                            flag = true;
                        }
                        if (!string.IsNullOrWhiteSpace(word) && !flag)
                        {
                            list.AddRange(SubTokenize(word, line, column));
                        }
                        else if (flag)
                        {
                            str2 = str2 + str3 + " ";
                        }
                        column += word.Length + 1;
                        if (flag2)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        str2 = str2.TrimEnd(new char[0]);
                        Token token2 = new Token
                        {
                            text = str2,
                            line = line,
                            column = column
                        };
                        list.Add(token2);
                    }
                }
                line++;
            }
            return list;
        }
    }
}
