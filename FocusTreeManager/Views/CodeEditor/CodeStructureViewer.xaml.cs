using FocusTreeManager.CodeStructures;
using FocusTreeManager.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Views.CodeEditor
{
    public struct CodeElement
    {
        public string Name { get; set; }
        public int Occurence { get; set; }
        public List<CodeElement> Childrens { get; set; }
    }

    public partial class CodeStructureViewer : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<CodeElement> CodeBlocks { get; }

        public CodeEditor LinkedEditor { get; set; }

        public CodeStructureViewer()
        {
            InitializeComponent();
            CodeBlocks = new ObservableCollection<CodeElement>();
        }

        public ScriptErrorLogger SetupViewer(string script)
        {
            List<string> AllAddedNames = new List<string>();
            CodeBlocks.Clear();
            Script newscript = new Script();
            newscript.Analyse(script);
            if (newscript.Logger.hasErrors())
            {
                return newscript.Logger;
            }
            foreach (ICodeStruct codeStruct in newscript.Code)
            {
                Assignation item = (Assignation) codeStruct;
                CodeElement element = new CodeElement
                {
                    Occurence = AllAddedNames.Count(n => n == item.Assignee),
                    Name = item.Assignee
                };
                CodeBlock block = item.Value as CodeBlock;
                element.Childrens = block != null ? RunInBlock(block, AllAddedNames) : 
                    new List<CodeElement>();
                AllAddedNames.Add(element.Name);
                CodeBlocks.Add(element);
            }
            OnPropertyChanged("CodeBlocks");
            return null;
        }

        private static List<CodeElement> RunInBlock(CodeBlock block, ICollection<string> AllAddedNames)
        {
            List<CodeElement> newlist = new List<CodeElement>();
            foreach (ICodeStruct codeStruct in block.Code.Where(t => t is Assignation))
            {
                Assignation item = (Assignation) codeStruct;
                CodeElement element = new CodeElement
                {
                    Occurence = AllAddedNames.Count(n => n == item.Assignee),
                    Name = item.Assignee
                };
                CodeBlock codeBlock = item.Value as CodeBlock;
                element.Childrens = codeBlock != null ? RunInBlock(codeBlock, AllAddedNames) : 
                    new List<CodeElement>();
                AllAddedNames.Add(element.Name);
                newlist.Add(element);
            }
            return newlist;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //If not a long hold click
            if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 2) return;
            TreeViewItem treeViewItem = UiHelper.FindVisualParent<TreeViewItem>
                (e.OriginalSource as DependencyObject, this);
            //If there is no tree view to be found
            if (treeViewItem == null) return;
            CodeElement codeBlock = (CodeElement)treeViewItem.DataContext;
            LinkedEditor.Select(codeBlock.Name, codeBlock.Occurence);
            e.Handled = true;
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
