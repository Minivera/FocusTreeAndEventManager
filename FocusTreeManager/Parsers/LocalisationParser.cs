using System.Collections.Generic;
using System.Text;
using System.Linq;
using FocusTreeManager.Model;
using System.IO;
using System.Text.RegularExpressions;
using FocusTreeManager.DataContract;
using FocusTreeManager.Model.TabModels;

namespace FocusTreeManager.Parsers
{
    public class LocalisationParser
    {
        public static string ParseLocalizationFileForCompare(string filename)
        {
            return !File.Exists(filename) ? "" : 
                ParseLocalizationForCompare(CreateLocaleFromFile(filename));
        }

        public static string ParseLocalizationForCompare(LocalisationModel model)
        {
            LocalisationContainer container = new LocalisationContainer(model);
            return Parse(container.LocalisationMap.ToList(), container.LanguageName);
        }

        public static Dictionary<string, string> ParseEverything(List<LocalisationContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (LocalisationContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                fileList[ID] = Parse(container.LocalisationMap.ToList(), container.LanguageName);
            }
            return fileList;
        }

        private static string Parse(IEnumerable<LocaleContent> iMap, string language)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(language + ":");
            foreach (LocaleContent pair in iMap)
            {
                text.AppendLine(" " + pair.Key + ":0 " + "\"" + pair.Value + "\"");
            }
            return text.ToString();
        }

        public static LocalisationModel CreateLocaleFromFile(string fileName)
        {
            LocalisationModel container = new LocalisationModel(
                Path.GetFileNameWithoutExtension(fileName));
            IEnumerable<string> lines = File.ReadLines(fileName);
            bool firstline = true;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (firstline)
                {
                    container.LanguageName = line.Replace(":", "").Trim();
                    firstline = false;
                }
                else
                {
                    LocaleModel content = new LocaleModel()
                    {
                        Key = line.Split(':')[0].Trim(),
                        Value = Regex.Match(line.Split(':')[1], "\"([^\"]*)\"").Groups[1].Value.Trim()
                    };
                    container.LocalisationMap.Add(content);
                }
            }
            return container;
        }
    }
}