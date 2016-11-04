using FocusTreeManager.CodeStructures;
using FocusTreeManager.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FocusTreeManager.Views.CodeEditor
{
    public struct CodeElement
    {
        public string Name { get; set; }
        public int Line { get; set; }
        public List<CodeElement> Childrens { get; set; }
    }

    public partial class CodeStructureViewer : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<CodeElement> codeBlocks;

        public ObservableCollection<CodeElement> CodeBlocks
        {
            get
            {
                return codeBlocks;
            }
        }

        public CodeEditor LinkedEditor { get; set; }

        public CodeStructureViewer()
        {
            InitializeComponent();
            codeBlocks = new ObservableCollection<CodeElement>();
        }

        public void SetupViewer(string script)
        {
            CodeBlocks.Clear();
            Script newscript = new Script();
            newscript.Analyse(script);
            foreach (CodeStructures.Assignation item in newscript.Code)
            {
                CodeElement element = new CodeElement();
                element.Line = item.Line;
                element.Name = item.Assignee;
                if (item.Value is CodeBlock)
                {
                    element.Childrens = RunInBlock(item.Value as CodeBlock);
                }
                else
                {
                    element.Childrens = new List<CodeElement>();
                }
                CodeBlocks.Add(element);
            }
            OnPropertyChanged("CodeBlocks");
        }

        private List<CodeElement> RunInBlock(CodeBlock block)
        {
            List<CodeElement> newlist = new List<CodeElement>();
            foreach (CodeStructures.Assignation item in block.Code)
            {
                CodeElement element = new CodeElement();
                element.Line = item.Line;
                element.Name = item.Assignee;
                if (item.Value is CodeBlock)
                {
                    element.Childrens = RunInBlock(item.Value as CodeBlock);
                }
                else
                {
                    element.Childrens = new List<CodeElement>();
                }
                newlist.Add(element);
            }
            return newlist;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                TreeViewItem treeViewItem = UiHelper.FindVisualParent<TreeViewItem>
                                        (e.OriginalSource as DependencyObject, this);
                if (treeViewItem != null)
                {
                    CodeElement codeBlock = (CodeElement)treeViewItem.DataContext;
                    LinkedEditor.Select(codeBlock.Name, codeBlock.Line);
                    e.Handled = true;
                }
            }
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
