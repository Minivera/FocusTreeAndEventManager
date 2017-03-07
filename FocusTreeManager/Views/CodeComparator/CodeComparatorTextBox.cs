using DiffPlex.DiffBuilder.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FocusTreeManager.Views.CodeComparator
{
    public class CodeComparatorTextBox : StackPanel
    {
        private const int MAX_LINE_TO_SKIP = 25;

        private const string ADDED_LINE_TEXT = "+ ";

        private readonly Brush ADDED_LINE_COLOR = (SolidColorBrush)
                        new BrushConverter().ConvertFrom("#00c853");

        private const string CHANGED_LINE_TEXT = "* ";

        private readonly Brush CHANGED_LINE_COLOR = (SolidColorBrush)
                        new BrushConverter().ConvertFrom("#ffd600");

        private const string DELETED_LINE_TEXT = "- ";

        private readonly Brush DELETED_LINE_COLOR = (SolidColorBrush)
                        new BrushConverter().ConvertFrom("#d50000");

        private const string NOTHING_LINE_TEXT = "  ";

        public static readonly DependencyProperty DiffModelProperty =
        DependencyProperty.Register("DiffModel", typeof(DiffPaneModel), 
            typeof(CodeComparatorTextBox), new UIPropertyMetadata(null, DiffModelChanged));
        
        public DiffPaneModel DiffModel
        {
            get { return (DiffPaneModel)GetValue(DiffModelProperty); }
            set { SetValue(DiffModelProperty, value); }
        }

        private static void DiffModelChanged(DependencyObject sender, 
                                             DependencyPropertyChangedEventArgs e)
        {
            CodeComparatorTextBox textbox = sender as CodeComparatorTextBox;
            textbox?.CreateFlowDocumentFromDiff(e.NewValue as DiffPaneModel);
        }

        private void CreateFlowDocumentFromDiff(DiffPaneModel model)
        {
            Orientation = Orientation.Vertical;
            Background = Brushes.White;
            int row = 1;
            bool skipping = false;
            int skipCount = 0;
            foreach (DiffPiece diff in model.Lines)
            {
                //If the current element has no change, is not skipping and one 
                //of the const next is not changed and has passed at least enough rows
                if ((diff.Type == ChangeType.Unchanged ||
                    diff.Type == ChangeType.Imaginary) && !skipping 
                    && row > MAX_LINE_TO_SKIP
                    && NoChangeForNextTenRows(diff, model) )
                {
                    //Start skipping
                    skipping = true;
                    skipCount = 0;
                }
                else if (skipping && diff.Type != ChangeType.Unchanged &&
                         diff.Type != ChangeType.Imaginary)
                {
                    //Stop skipping and write the number of skipped lines
                    //TODO: Add language support
                    skipping = false;
                    Children.Add(CreateDocumentRow(row, "Skipped " + skipCount.ToString() + " lines."));
                }
                if (skipping)
                {
                    skipCount++;
                }
                else
                {
                    switch (diff.Type)
                    {
                        case ChangeType.Inserted:
                            Children.Add(CreateDocumentRow(row, ADDED_LINE_TEXT + diff.Text,
                                ADDED_LINE_COLOR));
                            break;
                        case ChangeType.Modified:
                            Children.Add(CreateDocumentRow(row, CHANGED_LINE_TEXT + diff.Text,
                                CHANGED_LINE_COLOR));
                            break;
                        case ChangeType.Deleted:
                            Children.Add(CreateDocumentRow(row, DELETED_LINE_TEXT + diff.Text,
                                DELETED_LINE_COLOR));
                            break;
                        case ChangeType.Unchanged:
                            Children.Add(CreateDocumentRow(row, NOTHING_LINE_TEXT + diff.Text));
                            break;
                        case ChangeType.Imaginary:
                            Children.Add(CreateDocumentRow(row, NOTHING_LINE_TEXT + diff.Text));
                            break;
                    }
                }
                row++;
            }
            if (skipping)
            {
                //Write the final number of line we skipped
                //TODO: Add language support
                Children.Add(CreateDocumentRow(row, "Skipped " + skipCount + " lines."));
            }
        }

        private static bool NoChangeForNextTenRows(DiffPiece current, DiffPaneModel model)
        {
            int i = 0;
            foreach (DiffPiece diff in model.Lines.SkipWhile(x => x != current).Skip(1))
            {
                if (diff.Type != ChangeType.Unchanged &&
                    diff.Type != ChangeType.Imaginary)
                {
                    return false;
                }
                if(i >= MAX_LINE_TO_SKIP)
                {
                    return true;
                }
                i++;
            }
            return true;
        }

        private static UIElement CreateDocumentRow(int row, string text, Brush color = null)
        {
            StackPanel container = new StackPanel {Orientation = Orientation.Horizontal};
            container.Children.Add(new TextBlock()
            {
                Text = row.ToString(),
                Background = Brushes.Gray,
                Width = 40,
                TextAlignment = TextAlignment.Right
            });
            TextBlock block = new TextBlock()
            {
                Text = text,
                FontFamily = new FontFamily("Monaco"),
                FontSize = 12,
                Foreground = Brushes.Black
            };
            if (color != null)
            {
                block.Background = color;
            }
            container.Children.Add(block);
            return container;
        }
    }
}
