//Modified version of http://syntaxhighlightbox.codeplex.com

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using FocusTreeManager.Helper;
using FocusTreeManager.CodeStructures.CodeEditor;
using System.ComponentModel;

namespace FocusTreeManager.Views.CodeEditor
{
	public partial class CodeEditor : TextBox, INotifyPropertyChanged
    {
        public double LineHeight
        {
			get
            {
                return lineHeight;
            }
			set
            {
				if (value != lineHeight)
                {
					lineHeight = value;
					blockHeight = MaxLineCountInBlock * value;
					TextBlock.SetLineStackingStrategy(this, LineStackingStrategy.BlockLineHeight);
					TextBlock.SetLineHeight(this, lineHeight);
				}
			}
		}

		public int MaxLineCountInBlock
        {
			get
            {
                return maxLineCountInBlock;
            }
			set
            {
				maxLineCountInBlock = value > 0 ? value : 0;
				blockHeight = value * LineHeight;
			}
		}

		private DrawingControl renderCanvas;

		private DrawingControl lineNumbersCanvas;

		private ScrollViewer scrollViewer;

        private double lineHeight;

		private int totalLineCount;

		private List<InnerTextBlock> blocks;

		private double blockHeight;

		private int maxLineCountInBlock;

		public CodeEditor()
        {
			InitializeComponent();
			MaxLineCountInBlock = 100;
			LineHeight = FontSize * 1.3;
			totalLineCount = 1;
			blocks = new List<InnerTextBlock>();
			Loaded += (s, e) => {
                ApplyTemplate();
                renderCanvas = (DrawingControl)Template.FindName("PART_RenderCanvas", this);
                lineNumbersCanvas = (DrawingControl)Template.FindName("PART_LineNumbersCanvas", this);
				scrollViewer = (ScrollViewer)Template.FindName("PART_ContentHost", this);
                lineNumbersCanvas.Width = GetFormattedTextWidth(string.Format("{0:0000}", 
                    totalLineCount)) + 5;
				scrollViewer.ScrollChanged += OnScrollChanged;
				InvalidateBlocks(0);
				InvalidateVisual();
			};
			SizeChanged += (s, e) => {
                if (e.HeightChanged == false)
                {
                    return;
                }
				UpdateBlocks();
				InvalidateVisual();
			};
			TextChanged += (s, e) => {
				UpdateTotalLineCount();
				InvalidateBlocks(e.Changes.First().Offset);
				InvalidateVisual();
			};
        }

		protected override void OnRender(DrawingContext drawingContext)
        {
			DrawBlocks();
			base.OnRender(drawingContext);
		}

		private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
			if (e.VerticalChange != 0)
            {
                UpdateBlocks();
            }
			InvalidateVisual();
		}

        private void UpdateTotalLineCount()
        {
			totalLineCount = CodeHelper.GetLineCount(Text);
		}

		private void UpdateBlocks()
        {
			if (blocks.Count == 0)
            {
                return;
            }
			// While something is visible after last block
			while (!blocks.Last().IsLast && blocks.Last().Position.Y + blockHeight - VerticalOffset < ActualHeight)
            {
				int firstLineIndex = blocks.Last().LineEndIndex + 1;
				int lastLineIndex = firstLineIndex + maxLineCountInBlock - 1;
				lastLineIndex = lastLineIndex <= totalLineCount - 1 ? lastLineIndex : totalLineCount - 1;
				int fisrCharIndex = blocks.Last().CharEndIndex + 1;
				int lastCharIndex = CodeHelper.GetLastCharIndexFromLineIndex(Text, lastLineIndex);
				if (lastCharIndex <= fisrCharIndex)
                {
					blocks.Last().IsLast = true;
					return;
				}
				InnerTextBlock block = new InnerTextBlock(
					fisrCharIndex,
					lastCharIndex,
					blocks.Last().LineEndIndex + 1,
					lastLineIndex,
					LineHeight);
				block.RawText = block.GetSubString(Text);
				block.LineNumbers = GetFormattedLineNumbers(block.LineStartIndex, block.LineEndIndex);
				blocks.Add(block);
				FormatBlock(block, blocks.Count > 1 ? blocks[blocks.Count - 2] : null);
			}
		}

		private void InvalidateBlocks(int changeOffset)
        {
			InnerTextBlock blockChanged = null;
			for (int i = 0; i < blocks.Count; i++)
            {
				if (blocks[i].CharStartIndex <= changeOffset && changeOffset <= blocks[i].CharEndIndex + 1)
                {
					blockChanged = blocks[i];
					break;
				}
			}
			if (blockChanged == null && changeOffset > 0)
            {
                blockChanged = blocks.Last();
            }
			int fvline = blockChanged != null ? blockChanged.LineStartIndex : 0;
			int lvline = GetIndexOfLastVisibleLine();
			int fvchar = blockChanged != null ? blockChanged.CharStartIndex : 0;
			int lvchar = CodeHelper.GetLastCharIndexFromLineIndex(Text, lvline);
			if (blockChanged != null)
            {
                blocks.RemoveRange(blocks.IndexOf(blockChanged), blocks.Count - blocks.IndexOf(blockChanged));
            }
			int localLineCount = 1;
			int charStart = fvchar;
			int lineStart = fvline;
			for (int i = fvchar; i < Text.Length; i++)
            {
				if (Text[i] == '\n')
                {
					localLineCount += 1;
				}
				if (i == Text.Length - 1)
                {
					string blockText = Text.Substring(charStart);
					InnerTextBlock block = new InnerTextBlock(
						charStart,
						i, lineStart,
						lineStart + CodeHelper.GetLineCount(blockText) - 1,
						LineHeight);
					block.RawText = block.GetSubString(Text);
					block.LineNumbers = GetFormattedLineNumbers(block.LineStartIndex, block.LineEndIndex);
					block.IsLast = true;
					foreach (InnerTextBlock b in blocks)
                    {
                        if (b.LineStartIndex == block.LineStartIndex)
                        {
                            throw new Exception();
                        }
                    }
					blocks.Add(block);
					FormatBlock(block, blocks.Count > 1 ? blocks[blocks.Count - 2] : null);
					break;
				}
				if (localLineCount > maxLineCountInBlock)
                {
					InnerTextBlock block = new InnerTextBlock(
						charStart,
						i,
						lineStart,
						lineStart + maxLineCountInBlock - 1,
						LineHeight);
					block.RawText = block.GetSubString(Text);
					block.LineNumbers = GetFormattedLineNumbers(block.LineStartIndex, block.LineEndIndex);
                    foreach (InnerTextBlock b in blocks)
                    {
                        if (b.LineStartIndex == block.LineStartIndex)
                        {
                            throw new Exception();
                        }
                    }
                    blocks.Add(block);
					FormatBlock(block, blocks.Count > 1 ? blocks[blocks.Count - 2] : null);
					charStart = i + 1;
					lineStart += maxLineCountInBlock;
					localLineCount = 1;
					if (i > lvchar)
                    {
                        break;
                    }
				}
			}
		}

		private void DrawBlocks()
        {
			if (!IsLoaded || renderCanvas == null || lineNumbersCanvas == null)
            {
                return;
            }
			var dc = renderCanvas.GetContext();
			var dc2 = lineNumbersCanvas.GetContext();
			for (int i = 0; i < blocks.Count; i++)
            {
				InnerTextBlock block = blocks[i];
				Point blockPos = block.Position;
				double top = blockPos.Y - VerticalOffset;
				double bottom = top + blockHeight;
				if (top < ActualHeight && bottom > 0)
                {
					try
                    {
					    dc.DrawText(block.FormattedText, new Point(2 - 
                            HorizontalOffset, block.Position.Y - VerticalOffset));
					    lineNumbersCanvas.Width = GetFormattedTextWidth(string.Format("{0:0000}", 
                            totalLineCount)) + 5;
						dc2.DrawText(block.LineNumbers, new Point(lineNumbersCanvas.ActualWidth, 1 + 
                            block.Position.Y - VerticalOffset));
					}
                    catch
                    {
                        //Strange exception with large Copy
					}
				}
			}
			dc.Close();
		    dc2.Close();
		}

		/// <summary>
		/// Returns the index of the first visible text line.
		/// </summary>
		public int GetIndexOfFirstVisibleLine()
        {
			int guessedLine = (int)(VerticalOffset / lineHeight);
			return guessedLine > totalLineCount ? totalLineCount : guessedLine;
		}

		/// <summary>
		/// Returns the index of the last visible text line.
		/// </summary>
		public int GetIndexOfLastVisibleLine()
        {
			double height = VerticalOffset + ViewportHeight;
			int guessedLine = (int)(height / lineHeight);
			return guessedLine > totalLineCount - 1 ? totalLineCount - 1 : guessedLine;
		}

		/// <summary>
		/// Formats and Highlights the text of a block.
		/// </summary>
		private void FormatBlock(InnerTextBlock currentBlock, InnerTextBlock previousBlock)
        {
			currentBlock.FormattedText = GetFormattedText(currentBlock.RawText);
            Dispatcher.Invoke(() => {
                CodeEditorContent.Instance.Highlight(currentBlock.FormattedText);
                currentBlock.Code = -1;
            });
        }

		/// <summary>
		/// Returns a formatted text object from the given string
		/// </summary>
		private FormattedText GetFormattedText(string text)
        {
			FormattedText ft = new FormattedText(
				text,
				System.Globalization.CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
				FontSize,
				Brushes.White);
			ft.Trimming = TextTrimming.None;
			ft.LineHeight = lineHeight;
			return ft;
		}

		/// <summary>
		/// Returns a string containing a list of numbers separated with newlines.
		/// </summary>
		private FormattedText GetFormattedLineNumbers(int firstIndex, int lastIndex)
        {
			string text = "";
			for (int i = firstIndex + 1; i <= lastIndex + 1; i++)
            {
                text += i.ToString() + "\n";
            }
			text = text.Trim();
			FormattedText ft = new FormattedText(
				text,
				System.Globalization.CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
				FontSize,
				new SolidColorBrush(Color.FromRgb(0x21, 0xA1, 0xD8)));
			ft.Trimming = TextTrimming.None;
			ft.LineHeight = lineHeight;
			ft.TextAlignment = TextAlignment.Right;
			return ft;
		}

		/// <summary>
		/// Returns the width of a text once formatted.
		/// </summary>
		private double GetFormattedTextWidth(string text)
        {
			FormattedText ft = new FormattedText(
				text,
				System.Globalization.CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
				FontSize,
				Brushes.White);
			ft.Trimming = TextTrimming.None;
			ft.LineHeight = lineHeight;
			return ft.Width;
		}

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
