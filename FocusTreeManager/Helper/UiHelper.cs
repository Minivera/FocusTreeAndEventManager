using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FocusTreeManager.Helper
{
    /// <summary>
    /// Helper methods for UI-related tasks.
    /// </summary>
    public static class UiHelper
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static T FindVisualParent<T>(DependencyObject depObj, 
            FrameworkElement HighestParent) where T : DependencyObject
        {
            if (depObj != null && depObj != HighestParent)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(depObj);
                if (parent != null && parent is T)
                {
                    return (T)parent;
                }
                return FindVisualParent<T>(parent, HighestParent);
            }
            return null;
        }

        public static bool RessourceExists(string ressourceName)
        {
            foreach (string item in GetResourceNames())
            {
                if (item.Contains(ressourceName))
                {
                    return true;
                }
            }
            return false;
        }

        private static string[] GetResourceNames()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resName = assembly.GetName().Name + ".g.resources";
            using (var stream = assembly.GetManifestResourceStream(resName))
            {
                using (var reader = new System.Resources.ResourceReader(stream))
                {
                    return reader.Cast<DictionaryEntry>().Select(entry =>
                             (string)entry.Key).ToArray();
                }
            }
        }
    }
}
