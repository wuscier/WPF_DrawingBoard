using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPF_DrawingBoard.Thumbs
{
    class MoveThumb:Thumb
    {
        private const double LineThickness = 15;

        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;
            FrameworkElement parent = designerItem?.Parent as FrameworkElement;

            if (designerItem != null)
            {
                double width = designerItem.ActualWidth;
                double height = designerItem.ActualHeight;

                if (parent != null)
                {
                    double parentWidth = parent.ActualWidth;
                    double parentHeight = parent.ActualHeight;

                    double left = Canvas.GetLeft(designerItem);
                    double top = Canvas.GetTop(designerItem);

                    double targetLeft = left + e.HorizontalChange;

                    if (targetLeft < 0)
                    {
                        targetLeft = LineThickness;
                    }

                    if (targetLeft + width > parentWidth)
                    {
                        targetLeft = parentWidth - width - LineThickness;
                    }
                    double targetTop = top + e.VerticalChange;

                    if (targetTop < 0)
                    {
                        targetTop = LineThickness;
                    }

                    if (targetTop + height > parentHeight)
                    {
                        targetTop = parentHeight - height - LineThickness;
                    }

                    Canvas.SetLeft(designerItem, targetLeft);
                    Canvas.SetTop(designerItem, targetTop);
                }
            }
        }
    }
}
