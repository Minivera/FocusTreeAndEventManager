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

        public static ScriptContainer CreateScriptFromFile(string fileName)
        {
            ScriptContainer container = new ScriptContainer(Path.GetFileNameWithoutExtension(fileName));
            Script newScript = new Script();
            newScript.Analyse(File.ReadAllText(fileName));
            container.InternalScript = newScript;
            return container;
        }
    }
}