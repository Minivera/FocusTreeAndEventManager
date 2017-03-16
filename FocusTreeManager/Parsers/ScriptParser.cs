using System.Collections.Generic;
using System.Text;
using System.IO;
using FocusTreeManager.CodeStructures;
using FocusTreeManager.DataContract;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Parsers
{
    public class ScriptParser
    {
        public static string ParseScriptFileForCompare(string filename)
        {
            return !File.Exists(filename) ? "" : 
                ParseScriptForCompare(CreateScriptFromFile(filename));
        }

        public static string ParseScriptForCompare(ScriptModel model)
        {
            ScriptContainer container1 = new ScriptContainer(model.VisibleName)
            {
                FileInfo = model.FileInfo,
                InternalScript = model.InternalScript
            };
            return Parse(container1.InternalScript);
        }

        public static Dictionary<string, string> ParseEverything(List<ScriptContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (ScriptContainer container in Containers)
            {
                fileList[container.ContainerID] = Parse(container.InternalScript);
            }
            return fileList;
        }

        private static string Parse(ICodeStruct script)
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