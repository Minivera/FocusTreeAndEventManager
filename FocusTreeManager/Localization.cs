using FocusTreeManager.Model;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FocusTreeManager
{
    class Localization
    {
        const string LOCALE_FILE_STRING = "/FocusTreeManager;component/Languages/";

        public Localization()
        {

        }

        public string getLanguageFile()
        {
            return LOCALE_FILE_STRING + ConfigurationManager.AppSettings["Language"] + ".xaml";
        }   

        public void setLanguage(string Language)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            configFile.AppSettings.Settings["Language"].Value = Language;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
