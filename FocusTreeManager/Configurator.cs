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
            return LOCALE_FILE_STRING + FocusTreeManager.Properties.Settings.Default.Language + ".xaml";
        }  
        
        static public string getLanguage()
        {
            return ConfigurationManager.AppSettings["Language"];
        } 

        static public void setLanguage(string Language)
        {
            FocusTreeManager.Properties.Settings.Default.Language = Language;
            FocusTreeManager.Properties.Settings.Default.Save();
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
            FocusTreeManager.Properties.Settings.Default.Path = path;
            FocusTreeManager.Properties.Settings.Default.Save();
        }

        static public string getGamePath()
        {
            return FocusTreeManager.Properties.Settings.Default.Path;
        }

        static public void setFirstStart()
        {
            FocusTreeManager.Properties.Settings.Default.IsFirstStart = true;
            FocusTreeManager.Properties.Settings.Default.Save();
        }

        static public bool getFirstStart()
        {
            return FocusTreeManager.Properties.Settings.Default.IsFirstStart;
        }

        static public void setScripterPreference(string preference)
        {
            FocusTreeManager.Properties.Settings.Default.ScripterPreference = preference;
            FocusTreeManager.Properties.Settings.Default.Save();
        }

        static public string getScripterPreference()
        {
            return FocusTreeManager.Properties.Settings.Default.ScripterPreference;
        }

        static public void setEditorShowStruct(bool preference)
        {
            FocusTreeManager.Properties.Settings.Default.EditorShowStruct = preference;
            FocusTreeManager.Properties.Settings.Default.Save();
        }

        static public bool getEditorShowStruct()
        {
            return FocusTreeManager.Properties.Settings.Default.EditorShowStruct;
        }

        static public void setEditorShowPlan(bool preference)
        {
            FocusTreeManager.Properties.Settings.Default.EditorShowPlan = preference;
            FocusTreeManager.Properties.Settings.Default.Save();
        }

        static public bool getEditorShowPlan()
        {
            return FocusTreeManager.Properties.Settings.Default.EditorShowPlan;
        }
    }
}
