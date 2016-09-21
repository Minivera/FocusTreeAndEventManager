using FocusTreeManager.Model;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;

namespace FocusTreeManager
{
    public class LanguageSelector
    {
        public string Name { get; set; }

        public string FileName { get; set; }
    }

    static class Configurator
    {
        const string LOCALE_FILE_STRING = "/FocusTreeManager;component/Languages/";

        static public string getLanguageFile()
        {
            return LOCALE_FILE_STRING + ConfigurationManager.AppSettings["Language"] + ".xaml";
        }  
        
        static public string getLanguage()
        {
            return ConfigurationManager.AppSettings["Language"];
        } 

        static public void setLanguage(string Language)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            configFile.AppSettings.Settings["Language"].Value = Language;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        static public List<LanguageSelector> returnAllLanguages()
        {
            List<LanguageSelector> list = new List<LanguageSelector>();
            string Languages = FocusTreeManager.Properties.Resources.Languages;
            foreach (string language in Languages.Split(','))
            {
                list.Add(new LanguageSelector()
                {
                    FileName = language.Split(';')[0],
                    Name = language.Split(';')[1]
                });
            }
            return list;
        }

        static public void setGamePath(string path)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            configFile.AppSettings.Settings["Path"].Value = path;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        static public string getGamePath()
        {
            return ConfigurationManager.AppSettings["Path"] as string;
        }
    }
}
