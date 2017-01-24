using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using FocusTreeManager.Containers;
using FocusTreeManager.Model;
using System.IO;
using System.Text.RegularExpressions;
using FocusTreeManager.DataContract;

namespace FocusTreeManager.Parsers
{
    public class LocalisationParser
    {
        public static Dictionary<string, string> ParseEverything(List<LocalisationContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (LocalisationContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                ID += "_" + container.ShortName;
                fileList[ID] = Parse(container.LocalisationMap.ToList(), ID, container.ShortName);
            }
            return fileList;
        }

        private static string Parse(List<LocaleContent> iMap, string ID, string language)
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
                if (firstline)
                {
                    container.Shortname = line.Replace(":", "").Trim();
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