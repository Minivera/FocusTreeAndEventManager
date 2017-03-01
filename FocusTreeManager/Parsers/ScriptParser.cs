using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using System.IO;
using System.Text.RegularExpressions;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;

namespace FocusTreeManager.Parsers
{
    public class ScriptParser
    {
        public static string ParseScriptFileForCompare(string filename)
        {
            if (!File.Exists(filename))
            {
                return "";
            }
            return ParseScriptForCompare(CreateScriptFromFile(filename));
        }

        public static string ParseScriptForCompare(ScriptModel model)
        {
            ScriptContainer container1 = new ScriptContainer(model.VisibleName)
            {
                FileInfo = model.FileInfo,
                InternalScript = model.InternalScript
            };
            string iD = container1.ContainerID.Replace(" ", "_");
            return Parse(container1.InternalScript, iD);
        }

        public static Dictionary<string, string> ParseEverything(List<ScriptContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (ScriptContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                fileList[ID] = Parse(container.InternalScript, ID);
            }
            return fileList;
        }

        private static string Parse(Script script, string ID)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(script.Parse());
            return text.ToString();
        }

        public static ScriptModel CreateScriptFromFile(string fileName)
        {
            ScriptModel container = new ScriptModel(Path.GetFileNameWithoutExtension(fileName));
            Script newScript = new Script();
            newScript.Analyse(File.ReadAllText(fileName));
            container.InternalScript = newScript;
            return container;
        }
    }
}