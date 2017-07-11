using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FocusTreeManager.CodeStructures.CodeExceptions;

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

    public class Tokenizer
    {
        private static readonly char[] delimiters_array = { '=', '<', '>', '{', '}' };

        private const char comment_char = '#';

        public ScriptErrorLogger Logger { get; }

        public Tokenizer()
        {
            Logger = new ScriptErrorLogger();
        }

        public object GroupTokensByBlocks(List<Token> tokens, bool first = true)
        {
            List<SyntaxGroup> list = new List<SyntaxGroup>();
            //Check if the list contains anything and is the first recursive call
            if (!tokens.Any() && first)
            {
                return list;
            }
            //Check if the list contains any delimiters or can be considered a valid script
            if (!ContainsDelimiters(tokens) || DetectAnyErrors(tokens))
            {
                //if not, return the list as the operand, it is a list of text
                return tokens;
            }
            while (tokens.Any())
            {
                SyntaxGroup group = new SyntaxGroup();
                //First token should be the component
                try
                {
                    //Check if there is enough elements
                    if (tokens.Count < 3)
                    {
                        //If yes, incomplete expression
                        Logger.Errors.Add(new SyntaxError(tokens.First()?.text,
                            tokens.First()?.line,
                            tokens.First()?.column,
                            new IncompleteExpressionException()));
                        return null;
                    }
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
                        Logger.Errors.Add(new SyntaxError(group.Component?.text,
                            group.Component?.line,
                            group.Component?.column,
                            new OperatorExpectedException(tokens.First().text)));
                        //Load it anyway with = as the operator
                        group.Operator = new Token
                        {
                            text = "=",
                            line = group.Component.line,
                            column = group.Component.column 
                                     + group.Component.text.Length
                        };
                    }
                    //Third token should be the operand
                    switch (tokens.First().text)
                    {
                        case "{":
                            tokens.RemoveAt(0);
                            int closePos = getIndexOfClosingBracket(tokens) - 1;
                            if (closePos < 0)
                            {
                                Logger.Errors.Add(new SyntaxError(group.Component?.text,
                                    group.Component?.line,
                                    group.Component?.column,
                                    new ClosingBracketExpectedException()));
                                continue;
                            }
                            group.Operand = GroupTokensByBlocks(tokens.GetRange(0,
                                closePos), false);
                            tokens.RemoveRange(0, closePos + 1);
                            break;
                        case "}":
                            Logger.Errors.Add(new SyntaxError(group.Component?.text,
                                    group.Component?.line,
                                    group.Component?.column,
                                new OpeningBracketExpectedException(tokens.First().text)));
                            break;
                        case "\"":
                            //If we hit a string literal, malformed string
                            Logger.Errors.Add(new SyntaxError(group.Component?.text,
                                    group.Component?.line,
                                    group.Component?.column,
                                new StringLiteralException()));
                            break;
                        default:
                            //Check if it is empty
                            if (string.IsNullOrWhiteSpace(tokens.First().text))
                            {
                                //If yes, create and error
                                Logger.Errors.Add(new SyntaxError(group.Component?.text,
                                        group.Component?.line,
                                        group.Component?.column,
                                        new IncompleteExpressionException()));
                            }
                            group.Operand = tokens.First();
                            tokens.RemoveAt(0);
                            break;
                    }
                }
                catch (System.InvalidOperationException)
                {
                    //Critical incomplete expression, this will also log
                    Logger.Errors.Add(new SyntaxError(group.Component?.text,
                            group.Component?.line,
                            group.Component?.column,
                            new IncompleteExpressionException()));
                }
                list.Add(group);
            }
            return list;
        }

        private bool DetectAnyErrors(IReadOnlyCollection<Token> tokens)
        {
            //Check if there is at least three tokens
            if (tokens.Count < 3)
            {
                //If yes, incomplete expression
                Logger.Errors.Add(new SyntaxError(tokens.First()?.text,
                    tokens.First()?.line,
                    tokens.First()?.column,
                    new IncompleteExpressionException()));
                return true;
            }
            //Check if there is an opening bracket
            if (tokens.All(t => t.text != "{")) return false;
            {
                //Check if there is a closing bracket
                if (getIndexOfClosingBracket(tokens.SkipWhile(t => t.text != "{").Skip(1).ToList()) >= 0)
                    return false;
                Logger.Errors.Add(new SyntaxError(tokens.First().text,
                    tokens.First().line,
                    tokens.First().column,
                    new ClosingBracketExpectedException()));
                return true;
            }
        }

        public List<Token> Tokenize(string text, Script sender)
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
                        }
                        //Check if the current word if the start of a full string between quotes
                        if (!isStartedComment && word.Contains('"') && !word.Contains('\\'))
                        {
                            //If it is also the end
                            if (isFullyStringed || word.Count(f => f == '"') > 1)
                            {
                                //Create a local word up to the index of the quote
                                string localWord = word.Substring(0, word.LastIndexOf('"') + 1);
                                word = word.Substring(word.LastIndexOf('"') + 1);
                                //Create the token and skip to the next work
                                isFullyStringed = false;
                                fullString = fullString + localWord.TrimEnd();
                                Token token = new Token
                                {
                                    text = fullString,
                                    line = x,
                                    column = y - fullString.Length
                                };
                                list.Add(token);
                                y += fullString.Length;
                                //Add the remaining part of the word after the quote to the list
                                list.AddRange(SubTokenize(word, x, y));
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
                        else if (!string.IsNullOrWhiteSpace(word) && !isStartedComment)
                        {
                            //Sub tokenize the string, will cut by delimiters.
                            list.AddRange(SubTokenize(word, x, y));
                        }
                        y += item.Length + 1;
                        //If we did not start a comment during this line, kill
                        if (!item.Contains(comment_char) && !isStartedComment) continue;
                        //Check if the word contains a comment char (First time hit)
                        sender.AddWordToComment(x,
                            item.Contains(comment_char) ? item.Substring(item.IndexOf(comment_char))
                                : item);
                        //Start of a comment, continue, but end line afterwards
                        isStartedComment = true;
                    }
                    //If still fully string, but the line has ended
                    if (isFullyStringed)
                    {
                        fullString = fullString.TrimEnd();
                        Token token = new Token
                        {
                            text = fullString,
                            line = x,
                            column = y - fullString.Length
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
                list.Add(new Token
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
                list.Add(new Token
                {
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
        private static int getIndexOfClosingBracket(IReadOnlyList<Token> tokens)
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
            return noBrackets <=0 && i >= 1 ? i : -1;
        }

        private static bool ContainsDelimiters(IEnumerable<Token> tokens)
        {
            return tokens.Any(item => item.text.IndexOfAny(delimiters_array) != -1);
        }
    }
}
