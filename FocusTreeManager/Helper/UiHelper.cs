using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;

namespace FocusTreeManager.Helper
{
    /// <summary>
    /// Helper methods for UI-related tasks.
    /// </summary>
    public static class UiHelper
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) 
            where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                T children = child as T;
                if (children != null)
                {
                    yield return children;
                }
                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        public static T FindVisualParent<T>(DependencyObject depObj, FrameworkElement HighestParent) 
            where T : DependencyObject
        {
            while (true)
            {
                if (depObj == null || Equals(depObj, HighestParent)) return null;
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);
                T visualParent = parent as T;
                if (visualParent != null)
                {
                    return visualParent;
                }
                depObj = parent;
            }
        }

        public static bool RessourceExists(string ressourceName)
        {
            return GetResourceNames().Any(item => item.Contains(ressourceName));
        }

        private static IEnumerable<string> GetResourceNames()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            using (Stream stream = assembly.GetManifestResourceStream(resName))
            {
                if (stream == null) return null;
                using (ResourceReader reader = new ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>().Select(entry =>
                        (string) entry.Key).ToArray();
                }
            }
        }
    }
}
