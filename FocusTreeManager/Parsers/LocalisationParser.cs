using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FocusTreeManager.Containers;
using FocusTreeManager.Model;

namespace FocusTreeManager.Parsers
{
    public class LocalisationParser
    {
        public static Dictionary<string, string> ParseEverything(ObservableCollection<LocalisationContainer> Containers)
        {
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            foreach (LocalisationContainer container in Containers)
            {
                string ID = container.ContainerID.Replace(" ", "_");
                ID += "_" + container.ShortName;
                fileList[ID] = Parse(container.LocalisationMap, ID, container.ShortName);
            }
            return fileList;
        }

        private static string Parse(ObservableCollection<LocaleContent> iMap, string ID, string language)
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(language + ":");
            foreach (LocaleContent pair in iMap)
            {
                text.AppendLine(" " + pair.Key + ":0 " + "\"" + pair.Value + "\"");
            }
            return text.ToString();
        }
    }
}