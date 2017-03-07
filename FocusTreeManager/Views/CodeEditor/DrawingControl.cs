using System;
using System.Windows;
using System.Windows.Media;

namespace FocusTreeManager.Views.CodeEditor
{
	public class DrawingControl : FrameworkElement
    {
		private readonly VisualCollection visuals;

		private readonly DrawingVisual visual;

		public DrawingControl()
        {
			visual = new DrawingVisual();
            visuals = new VisualCollection(this) {visual};
        }

		public DrawingContext GetContext()
        {
			return visual.RenderOpen();
		}

        public DrawingVisual getVisual()
        {
            return visual;
        }

		protected override int VisualChildrenCount => visuals.Count;

        protected override Visual GetVisualChild(int index)
        {
			if (index < 0 || index >= visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
			return visuals[index];
		}
	}
}
