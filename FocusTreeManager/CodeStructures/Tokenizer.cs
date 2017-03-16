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
    /// Class that store all tokens as a group of something operator something else
    /// </summary>
    public class SyntaxGroup
    {
        public Token Component { get; set; }

        public Token Operator { get; set; }

        public object Operand { get; set; }
    }

    public static class Tokenizer
    {
        private static readonly char[] delimiters_array = { '=', '<', '>', '{', '}' };

        private const char comment_char = '#';

        public static object GroupTokensByBlocks(List<Token> tokens, bool first = true)
        {
            List<SyntaxGroup> list = new List<SyntaxGroup>();
            //Check if the list contains anything and is the first recursive call
            if (!tokens.Any() && first)
            {
                return list;
            }
            //Check if the list contains any delimiters
            if (!ContainsDelimiters(tokens))
            {
                //if not, return the list as the operand, it is a list of text
                return tokens;
            }
            while (tokens.Any())
            {
                SyntaxGroup group = new SyntaxGroup();
                //First token should be the component
                group.Component = tokens.First();
                tokens.Remove(tokens.First());
                //second token should be the operator
                if (tokens.First().text == "=" ||
                    tokens.First().text == "<" ||
                    tokens.First().text == ">")
                {
                    group.Operator = tokens.First();
                    tokens.RemoveAt(0);
                }
                else
                {
                    throw new SyntaxException(group.Component.text, 
                        tokens.First().line, tokens.First().column);
                }
                //Third token should be the operand
                if (tokens.First().text == "{")
                {
                    tokens.RemoveAt(0);
                    int closePos = getIndexOfClosingBracket(tokens) - 1;
                    group.Operand = GroupTokensByBlocks(tokens.GetRange(0,
                                    closePos), false);
                    tokens.RemoveRange(0, closePos + 1);
                }
                else if (tokens.First().text == "}")
                {
                    throw new SyntaxException(group.Component.text, 
                        tokens.First().line, tokens.First().column);
                }
                //Pure text
                else
                {
                    group.Operand = tokens.First();
                    tokens.RemoveAt(0);
                }
                list.Add(group);
            }
            return list;
        }

        public static List<Token> Tokenize(string text)
        {
            List<Token> list = new List<Token>();
            //Split by new lines
            int x = 1;
            foreach (string line in text.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    //Split by spaces
                    int y = 1;
                    bool isFullyStringed = false;
                    bool isStartedComment = false;
                    string fullString = "";
                    foreach (string item in line.Split(' ', '\t'))
                    {
                        string word = item;
                        if (item.Contains(comment_char))
                        {
                            word = item.Substring(0, item.IndexOf(comment_char));
                            //Start of a comment, continue, but end line afterwards
                            isStartedComment = true;
                        }
                        //Check if the current word if the start of a full string between quotes
                        if (word.Contains('"') && !word.Contains('\\'))
                        {
                            //If it is also the end
                            if (isFullyStringed || word.Count(f => f == '"') > 1)
                            {
                                //Create the token and skip to the next work
                                isFullyStringed = false;
                                fullString = fullString + word.TrimEnd();
                                Token token = new Token
                                {
                                    text = fullString,
                                    line = x,
                                    column = y
                                };
                                list.Add(token);
                                fullString = "";
                                continue;
                            }
                            //Otherwise, it is fully stringed
                            isFullyStringed = true;
                        }
                        if (isFullyStringed)
                        {
                            fullString += word + " ";
                        }
                        else if (!string.IsNullOrWhiteSpace(word))
                        {
                            //Sub tokenize the string, will cut by delimiters.
                            list.AddRange(SubTokenize(word, x, y));
                        }
                        y += item.Length + 1;
                        //If we started a comment during this line, kill
                        if (isStartedComment)
                        {
                            break;
                        }
                    }
                    //If still fully string, but the line has ended
                    if (isFullyStringed)
                    {
                        fullString = fullString.TrimEnd();
                        Token token = new Token
                        {
                            text = fullString,
                            line = x,
                            column = y
                        };
                        list.Add(token);
                    }
                }
                x++;
            }
            return list;
        }

        private static IEnumerable<Token> SubTokenize(string text, int line, int column)
        {
            List<Token> list = new List<Token>();
            //If the text contains any delimiters
            if (text.IndexOfAny(delimiters_array) != -1)
            {
                int y = column;
                string item = text.Substring(0, text.IndexOfAny(delimiters_array));
                //Sub tokenize the string before the delimiter.
                list.AddRange(SubTokenize(item, line, y));
                //Remove the added tokens from the string
                text = text.Substring(text.IndexOfAny(delimiters_array));
                y += item.Length;
                //Add the delimiter to the list
                list.Add(new Token()
                {
                    text = text.Substring(0, 1).Trim(),
                    line = line,
                    column = y
                });
                //Remove the delimiter from the string
                text = text.Substring(1);
                y++;
                //Sub tokenize the string after the delimiter.
                list.AddRange(SubTokenize(text, line, y));
            }
            //Otherwise, clean text
            else if(!string.IsNullOrWhiteSpace(text))
            {
                list.Add(new Token() {
                    text = text.Trim(),
                    line = line,
                    column = column
                });
            }
            return list;
        }

        /// <summary>
        /// Gets the position of the closing bracket in a list where the first bracket do not exist
        /// </summary>
        /// <param name="tokens">List of tokens to loop through</param>
        /// <returns>Position of the corresponding closing bracket, -1 if not found.</returns>
        private static int getIndexOfClosingBracket(List<Token> tokens)
        {
            int noBrackets = 1;
            int i = 0;
            while (noBrackets > 0 && i < tokens.Count)
            {
                switch (tokens[i].text)
                {
                    case "{":
                        noBrackets++;
                        break;
                    case "}":
                        noBrackets--;
                        break;
                }
                i++;
            }
            return i >= 1 ? i : -1;
        }

        private static bool ContainsDelimiters(IEnumerable<Token> tokens)
        {
            return tokens.Any(item => item.text.IndexOfAny(delimiters_array) != -1);
        }
    }
}
