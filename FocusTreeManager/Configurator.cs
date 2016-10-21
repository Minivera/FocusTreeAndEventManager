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
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return LOCALE_FILE_STRING + configFile.AppSettings.Settings["Language"].Value + ".xaml";
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
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return configFile.AppSettings.Settings["Path"].Value as string;
        }

        static public void setFirstStart()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            configFile.AppSettings.Settings["IsFirstStart"].Value = "true";
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        static public bool getFirstStart()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            string value = configFile.AppSettings.Settings["IsFirstStart"].Value as string;
            return value == "false" ? false : true;
        }

        static public void setScripterPreference(string preference)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            configFile.AppSettings.Settings["ScripterPreference"].Value = preference;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        static public string getScripterPreference()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return configFile.AppSettings.Settings["ScripterPreference"].Value as string;
        }
    }
}
