using FocusTreeManager.CodeStructures.CodeExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FocusTreeManager.CodeStructures
{
    /// <summary>
    /// Hold the whole script in any text file, a text file contains one or multiple assignations.
    /// </summary>
    [KnownType(typeof(Assignation))]
    [KnownType(typeof(CodeBlock))]
    [KnownType(typeof(CodeValue))]
    [DataContract(Name = "script_struct")]
    public class Script : ICodeStruct
    {
        [DataMember(Name = "code", Order = 0)]
        public List<ICodeStruct> Code { get; set; }

        [DataMember(Name = "comments", Order = 1)]
        public Dictionary<int, string> Comments { get; set; }

        public ScriptErrorLogger Logger { get; }

        public Script()
        {
            Code = new List<ICodeStruct>();
            Comments = new Dictionary<int, string>();
            Logger = new ScriptErrorLogger();
        }

        public void Analyse(string code, int line = -1)
        {
            Code.Clear();
            Tokenizer tokenizer = new Tokenizer();
            List<SyntaxGroup> list =
                tokenizer.GroupTokensByBlocks(tokenizer.Tokenize(code, this)) 
                as List<SyntaxGroup>;
            //Set this logger as the tokenizer logger
            Logger.Errors = tokenizer.Logger.Errors;
            //Check if there are errors.
            if (Logger.hasErrors())
            {
                return;
            }
            //Return if list is null
            if (list == null || !list.Any()) return;
            foreach (SyntaxGroup group in list)
            {
                try
                {
                    //Check if there is an assignation or code value at the end of the code
                    Assignation last = Code.LastOrDefault() as Assignation;
                    if (last != null)
                    {
                        //set its end line as this line's end - 1
                        last.EndLine = group.Component.line - 1;
                    }
                    Assignation tempo = new Assignation(1);
                    RecursiveCodeLog log = tempo.Analyse(group);
                    if (log != null)
                    {
                        Logger.Errors.Add(new SyntaxError(log.Message));
                        //We don't want to add broken code if possible
                        continue;
                    }
                    Code.Add(tempo);
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    Logger.Errors.Add(new SyntaxError("Unknown error in script"));
                }
            }
            //Check if there is an assignation or code value at the end of the code
            Assignation Verylast = Code.LastOrDefault() as Assignation;
            if (Verylast != null)
            {
                //Check if the last is Empty
                if (list.Last().Component.line == Verylast.Line)
                {
                    //Empty block, set the end line as starting line + 2
                    Verylast.EndLine = Verylast.Line + 2;
                }
                else
                {
                    //Set its end line as this line's end - 1
                    Verylast.EndLine = list.Last().Component.line - 1;
                }
            }
        }

        public string Parse(Dictionary<int, string> comments = null, int StartLevel = -1)
        {
            string content = "";
            //End here is the code is empty
            if (Code == null || !Code.Any())
            {
                return content;
            }
            //Make a local copy, so we do not remove the master comments
            Dictionary<int, string> localCopy = Comments?.ToDictionary(t => t.Key, t => t.Value);
            foreach (ICodeStruct item in Code)
            {
                try
                {
                    content += item.Parse(localCopy, StartLevel) + "\n";
                }
                catch (Exception)
                {
                    //TODO: Add language support
                    Logger.Errors.Add(new SyntaxError("Unknown error in script"));
                }
            }
            return content;
        }

        public CodeValue FindValue(string TagToFind)
        {
            //Run through all the code and return the found value or none.
            return Code.Select(item => item.FindValue(TagToFind))
                .FirstOrDefault(found => found != null);
        }

        public ICodeStruct Extract(string TagToFind)
        {
            foreach (ICodeStruct item in Code)
            {
                ICodeStruct found = item.Extract(TagToFind);
                //If nothing was found, return null
                if (found == null) continue;
                //Otherwise, extract
                if (Code.Contains(found))
                {
                    Code.Remove(found);
                }
                return found;
            }
            return null;
        }

        public Assignation FindAssignation(string TagToFind)
        {
            //Run through all the code and return the found value or none.
            return Code.Select(item => item.FindAssignation(TagToFind)).
                FirstOrDefault(found => found != null);
        }

        public List<ICodeStruct> FindAllValuesOfType<T>(string TagToFind)
        {
            List<ICodeStruct> founds = new List<ICodeStruct>();
            foreach (ICodeStruct item in Code)
            {
                founds.AddRange(item.FindAllValuesOfType<T>(TagToFind));
            }
            return founds;
        }

        public List<Assignation> FindAllAssignationsInRoot(string TagToFind)
        {
            return Code.OfType<Assignation>()
                .Where(copy => copy.Assignee == TagToFind).ToList();
        }

        public Script GetContentAsScript(string[] except,
            Dictionary<int, string> comments = null)
        {
            Script newScript = new Script();
            foreach (ICodeStruct codeStruct in Code)
            {
                Assignation item = (Assignation)codeStruct;
                if (!except.Contains(item.Assignee))
                {
                    newScript.Code.Add(item);
                }
            }
            Assignation firstLine = newScript.Code.Where(i => i is Assignation)
                .OrderBy(i => ((Assignation)i).Line).FirstOrDefault() as Assignation;
            Assignation lastLine = newScript.Code.Where(i => i is Assignation)
                .OrderBy(i => ((Assignation)i).Line).LastOrDefault() as Assignation;
            //If no lines were found
            if (firstLine == null || lastLine == null) return newScript;
            //Try to get the comments
            newScript.Comments = Comments.SkipWhile(c => c.Key < firstLine.Line)
                                         .TakeWhile(c => c.Key <= lastLine.Line)
                                         .ToDictionary(c => c.Key, c => c.Value);
            return newScript;
        }

        public void AddWordToComment(int line, string word)
        {
            if (Comments.ContainsKey(line))
            {
                Comments[line] += " " + word;
            }
            else
            {
                Comments[line] = word;
            }
        }

        public string TryParse(ICodeStruct block, string tag,
            Dictionary<int, string> comments = null, bool isMandatory = true)
        {
            Assignation found = block.FindAssignation(tag);
            if (found != null)
            {
                try
                {
                    return found.Value.Parse(comments);
                }
                catch (Exception)
                {
                    Logger.Errors.Add(new SyntaxError(tag, found.Line, null, 
                        new UnparsableTagException(tag)));
                }
            }
            if (isMandatory)
            {
                Logger.Errors.Add(new SyntaxError(tag, null, null, 
                    new MandatoryTagException(tag)));
            }
            return null;
        }
    }
}
