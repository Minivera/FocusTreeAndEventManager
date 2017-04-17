using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FocusTreeManager.Helper
{
    public enum LocalesSource
    {
        Default = 0,
        Tutorial = 1
    }

    public static class LocalizationHelper
    {
        private static readonly string[] TypeToValueAssoc = new string[] { "", "_Tutorial"};

        private const string LOCALE_FILE_STRING = "/FocusTreeManager;component/Languages/";

        public static string getValueForKey(string key, LocalesSource type = LocalesSource.Default)
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(LOCALE_FILE_STRING + Configurator.getLanguage() +
                                 TypeToValueAssoc[(int)type] + ".xaml", UriKind.Relative)
            };
            return resourceLocalization[key] as string;
        }

        public static ResourceDictionary getLocale(LocalesSource type = LocalesSource.Default)
        {
            return new ResourceDictionary
            {
                Source = new Uri(LOCALE_FILE_STRING + Configurator.getLanguage() +
                                 TypeToValueAssoc[(int)type] + ".xaml", UriKind.Relative)
            };
        }
    }
}
